using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using CareBridge.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Services
{
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityRepository _availabilityRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly TimeZoneInfo _clinicTimeZone;

        public DoctorAvailabilityService(IDoctorAvailabilityRepository availabilityRepository , 
            IAppointmentRepository appointmentRepository,
            IConfiguration configuration)
        {
            _availabilityRepository = availabilityRepository;
            _appointmentRepository = appointmentRepository;
            var timeZoneId = configuration["Clinic:TimeZoneId"]
                ?? throw new InvalidOperationException(
                    "Clinic time zone is not configured.");

            _clinicTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        // Add availability for multiple dates within the next 7 days
        public async Task<IReadOnlyList<DoctorAvailabilityResponseDto>>AddWeeklyAvailabilityAsync(
                Guid doctorId,
                IReadOnlyList<DoctorAvailabilityRequestDto> requests,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (doctorId == Guid.Empty)
                throw new BadRequestException("Doctor ID is required.");

            if (requests == null || requests.Count == 0)
                throw new BadRequestException(
                    "At least one availability schedule is required.");

            var today = GetClinicToday();
            var lastAllowedDate = today.AddDays(6);

            // Validate all requests before adding anything
            foreach (var request in requests)
            {
                ValidateTimeRange(request);

                if (request.AvailableDate < today ||
                    request.AvailableDate > lastAllowedDate)
                {
                    throw new BadRequestException(
                        $"Availability date must be between {today} and {lastAllowedDate}.");
                }
            }

            // Check duplicate/overlapping entries within this request
            for (int i = 0; i < requests.Count; i++)
            {
                for (int j = i + 1; j < requests.Count; j++)
                {
                    var first = requests[i];
                    var second = requests[j];

                    if (first.AvailableDate == second.AvailableDate &&
                        IsOverlapping(
                            first.StartTime,
                            first.EndTime,
                            second.StartTime,
                            second.EndTime))
                    {
                        throw new BadRequestException(
                            "The request contains overlapping availability times.");
                    }
                }
            }

            // Check existing database records
            foreach (var request in requests)
            {
                var hasOverlap =
                    await _availabilityRepository.HasOverlappingAvailabilityAsync(
                        doctorId,
                        request.AvailableDate,
                        request.StartTime,
                        request.EndTime,
                        null,
                        cancellationToken);

                if (hasOverlap)
                {
                    throw new BadRequestException(
                        $"Availability overlaps with an existing schedule on {request.AvailableDate}.");
                }
            }

            var availabilities = requests.Select(request =>
                new DoctorAvailability
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    AvailableDate = request.AvailableDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

            // Add all records and save them together
            foreach (var availability in availabilities)
            {
                await _availabilityRepository.AddAsync(
                    availability,
                    cancellationToken);
            }

            await _availabilityRepository.SaveChangesAsync(
                cancellationToken);

            return availabilities.Select(MapToResponse).ToList();
        }

        // Get availability for the next 7 days
        public async Task<IReadOnlyList<DoctorAvailabilityResponseDto>>
            GetWeeklyAvailabilityAsync(
                Guid doctorId,
                CancellationToken cancellationToken)
        {
            if (doctorId == Guid.Empty)
                throw new BadRequestException("Doctor ID is required.");

            var today = GetClinicToday();
            var endDate = today.AddDays(6);

            var availabilities =
                await _availabilityRepository.GetByDoctorAndDateRangeAsync(
                    doctorId,
                    today,
                    endDate,
                    cancellationToken);

            return availabilities
                .Select(MapToResponse)
                .ToList();
        }

        // Update an existing availability record
        public async Task<DoctorAvailabilityResponseDto> UpdateAvailabilityAsync(
            Guid doctorId,
            Guid availabilityId,
            DoctorAvailabilityRequestDto request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (doctorId == Guid.Empty || availabilityId == Guid.Empty)
                throw new BadRequestException(
                    "Doctor ID and availability ID are required.");

            ArgumentNullException.ThrowIfNull(request);

            ValidateTimeRange(request);

            var today = GetClinicToday();

            if (request.AvailableDate < today ||
                request.AvailableDate > today.AddDays(6))
            {
                throw new BadRequestException(
                    "Availability date must be within the next 7 days.");
            }

            var availability =
                await _availabilityRepository.GetByIdAsync(
                    availabilityId,
                    doctorId,
                    cancellationToken);

            if (availability == null)
                throw new NotFoundException(
                    "Availability record not found.");

            var hasOverlap =
                await _availabilityRepository.HasOverlappingAvailabilityAsync(
                    doctorId,
                    request.AvailableDate,
                    request.StartTime,
                    request.EndTime,
                    availabilityId,
                    cancellationToken);

            if (hasOverlap)
                throw new BadRequestException(
                    "This availability overlaps with another schedule.");

            // Protect existing pending and confirmed appointments
            var appointments =
                await _appointmentRepository.GetByDoctorAndDateAsync(
                    doctorId,
                    availability.AvailableDate,
                    cancellationToken);

            var invalidAppointment = appointments.Any(appointment =>
                IsBlocking(appointment.Status) &&
                appointment.StartTime < availability.EndTime &&
                appointment.EndTime > availability.StartTime &&
                (request.AvailableDate != availability.AvailableDate ||
                 appointment.StartTime < request.StartTime ||
                 appointment.EndTime > request.EndTime));

            if (invalidAppointment)
                throw new BadRequestException(
                    "This schedule cannot be changed because existing appointments would fall outside the new availability.");

            // If the date changes, also check bookings on the new date
            if (request.AvailableDate != availability.AvailableDate)
            {
                var newDateAppointments =
                    await _appointmentRepository.GetByDoctorAndDateAsync(
                        doctorId,
                        request.AvailableDate,
                        cancellationToken);

                var conflictsWithNewDate = newDateAppointments.Any(appointment =>
                    IsBlocking(appointment.Status) &&
                    request.StartTime < appointment.EndTime &&
                    request.EndTime > appointment.StartTime);

                if (conflictsWithNewDate)
                    throw new BadRequestException(
                        "The new availability overlaps with an existing appointment.");
            }

            availability.AvailableDate = request.AvailableDate;
            availability.StartTime = request.StartTime;
            availability.EndTime = request.EndTime;
            availability.UpdatedAt = DateTime.UtcNow;

            _availabilityRepository.Update(availability);

            await _availabilityRepository.SaveChangesAsync(
                cancellationToken);

            return MapToResponse(availability);
        }

        // Delete an availability record
        public async Task DeleteAvailabilityAsync(
            Guid doctorId,
            Guid availabilityId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (doctorId == Guid.Empty || availabilityId == Guid.Empty)
                throw new BadRequestException(
                    "Doctor ID and availability ID are required.");

            var availability =
                await _availabilityRepository.GetByIdAsync(
                    availabilityId,
                    doctorId,
                    cancellationToken);

            if (availability == null)
                throw new NotFoundException(
                    "Availability record not found.");

            var appointments =
                await _appointmentRepository.GetByDoctorAndDateAsync(
                    doctorId,
                    availability.AvailableDate,
                    cancellationToken);

            var hasBookings = appointments.Any(appointment =>
                IsBlocking(appointment.Status) &&
                appointment.StartTime < availability.EndTime &&
                appointment.EndTime > availability.StartTime);

            if (hasBookings)
                throw new BadRequestException(
                    "This availability cannot be deleted because appointments are already booked in this time range.");

            _availabilityRepository.Delete(availability);

            await _availabilityRepository.SaveChangesAsync(
                cancellationToken);
        }

        // Validate availability time range
        private static void ValidateTimeRange(
            DoctorAvailabilityRequestDto request)
        {
            if (request.StartTime >= request.EndTime)
                throw new BadRequestException(
                    "Start time must be earlier than end time.");
        }

        // Check time overlap
        private static bool IsOverlapping(
            TimeOnly firstStart,
            TimeOnly firstEnd,
            TimeOnly secondStart,
            TimeOnly secondEnd)
        {
            return firstStart < secondEnd &&
                   firstEnd > secondStart;
        }

        // Only these appointment statuses block schedule changes
        private static bool IsBlocking(AppointmentStatus status)
        {
            return status == AppointmentStatus.Pending ||
                   status == AppointmentStatus.Confirmed;
        }

        // Get today's date in the configured clinic timezone
        private DateOnly GetClinicToday()
        {
            var clinicNow = TimeZoneInfo.ConvertTime(
                DateTimeOffset.UtcNow,
                _clinicTimeZone);

            return DateOnly.FromDateTime(clinicNow.DateTime);
        }

        // Map entity to response DTO
        private static DoctorAvailabilityResponseDto MapToResponse(
            DoctorAvailability availability)
        {
            return new DoctorAvailabilityResponseDto
            {
                Id = availability.Id,
                AvailableDate = availability.AvailableDate,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsAvailable = availability.IsAvailable
            };
        }
    }
}
