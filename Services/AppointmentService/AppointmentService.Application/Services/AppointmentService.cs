using AppointmentService.Application.Constants;
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AutoMapper;
using CareBridge.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorAvailabilityRepository _doctorAvailabilityRepository;
        private readonly IDoctorUnavailablePeriodRepository _doctorUnavailablePeriodRepository;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _clinicTimeZone;

        public AppointmentService(IAppointmentRepository appointmentRepository,
            IDoctorAvailabilityRepository doctorAvailabilityRepository,
            IDoctorUnavailablePeriodRepository doctorUnavailablePeriodRepository,
            IMapper mapper,
            IConfiguration configuration
            
            )
        {
            _appointmentRepository = appointmentRepository;
            _doctorAvailabilityRepository = doctorAvailabilityRepository;
            _doctorUnavailablePeriodRepository = doctorUnavailablePeriodRepository;
            _mapper = mapper;

            var timeZoneId = configuration["Clinic:TimeZoneId"]
                ?? throw new InvalidOperationException(
                    "Clinic time zone is not configured.");

            _clinicTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        }

        public async Task<AppointmentResponseDto> BookAppointmentAsync(BookAppointmentRequestDto request,
            Guid patientId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(request);

            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty.", nameof(patientId));

            if (request.DoctorId == Guid.Empty)
                throw new ArgumentException("Doctor ID cannot be empty.", nameof(request.DoctorId));

            if (!Enum.IsDefined(request.AppointmentType))
            {
                throw new ArgumentException("Invalid appointment type .", nameof(request.AppointmentType)); 
            }

            // Validate schedule, date, availability, and booking conflicts
            var endTime = await ValidateRequestedSlotAsync(
                request.DoctorId,
                request.AppointmentDate,
                request.StartTime,
                cancellationToken);

            // Create the appointment entity
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = endTime,
                AppointmentType = request.AppointmentType,
                Status = AppointmentStatus.Pending,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // Save the appointment.
            await _appointmentRepository.AddAsync(appointment, cancellationToken);

            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            // Map the entity to the response DTO.
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString()
            }; 

        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(Guid appointmentId, Guid patientId, CancellationToken cancellationToken)
        {
            if (appointmentId == Guid.Empty)
            {
                throw new BadRequestException("Appointment ID cannot be empty.");
            }

            if (patientId == Guid.Empty)
            {
                throw new BadRequestException("Patient ID cannot be empty.");
            }

            var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(
                appointmentId,
                cancellationToken
                );

            if(appointment is null)
            {
                throw new NotFoundException("Appointment not found.");
            }

            if(appointment.PatientId != patientId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");
            }
            if(appointment.Status != AppointmentStatus.Pending && 
                appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new BadRequestException("Only pending or confirmed appointments can be cancelled.");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<AppointmentResponseDto> ConfirmAppointmentAsync(Guid appointmentId,
            Guid doctorId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(appointmentId == Guid.Empty)
            {
                throw new BadRequestException("Appointment ID cannot be empty.");
            }

            if(doctorId == Guid.Empty)
            {
                throw new BadRequestException("Doctor ID cannot be empty.");
            }

            var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(appointmentId, cancellationToken);

            if(appointment is null)
            {
                throw new NotFoundException("Appointment not found.");
            }
            if(appointment.DoctorId != doctorId)
            {
                throw new UnauthorizedAccessException("You are not authorized to confirm this appointment.");
            }
            if(appointment.Status != AppointmentStatus.Pending)
            {
                throw new BadRequestException("Only pending appointments can be confirmed.");
            }

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppointmentResponseDto>(appointment);

        }

        public async Task<AppointmentResponseDto> GetAppointmentById(Guid id,Guid patientId,
            CancellationToken cancellationToken)
        {
            if(id == Guid.Empty)
            {
                throw new BadRequestException("Appointment id cannot be empty");
            }
           var appointment = await _appointmentRepository.GetAppointmentById(id,patientId, cancellationToken);

            if(appointment is null)
            {
                throw new BadRequestException("Appointment not found.");
            }
            return _mapper.Map<AppointmentResponseDto>(appointment);

        }

        public async Task<IReadOnlyList<AvailableAppointmentSlotDto>> GetAvailableSlotsAsync(Guid doctorId,
            DateOnly date, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            var now = TimeZoneInfo.ConvertTime(
                         DateTimeOffset.UtcNow,
                         _clinicTimeZone);
            var today = DateOnly.FromDateTime(now.DateTime);
            var currentTime = TimeOnly.FromDateTime(now.DateTime);

            if (date < today)
            {
                return Array.Empty<AvailableAppointmentSlotDto>();
            }
            if (doctorId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Doctor ID cannot be empty.",
                    nameof(doctorId));
            }

            //find doctor schedule for the week

            var schedules = await _doctorAvailabilityRepository.GetByDoctorAndDayAsync(doctorId, date,
                cancellationToken);

            if (schedules.Count == 0)
            {
                return Array.Empty<AvailableAppointmentSlotDto>();
            }

            //doctor available or not

            var unavailablePeriods = await _doctorUnavailablePeriodRepository.GetOverlappingAsync(doctorId,
                date,
                cancellationToken
                );

            if (unavailablePeriods.Count > 0)
            {
                return Array.Empty<AvailableAppointmentSlotDto>();
            }

            //retrive existing appointments of the doctor based on date

            var appointments = await _appointmentRepository.GetByDoctorAndDateAsync(doctorId,
                date,
                cancellationToken
                );

            //pending and booked slots will blocked

            var blockingAppointments = appointments.Where(a => a.Status == AppointmentStatus.Pending ||
            a.Status == AppointmentStatus.Confirmed)
                .ToList();



            var availableSlots = new List<AvailableAppointmentSlotDto>();

            //Generate slots within each working interval

            foreach (var schedule in schedules)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!schedule.IsAvailable || schedule.StartTime >= schedule.EndTime)
                {
                    continue;
                }

                var slotStart = schedule.StartTime;

                while (slotStart.AddMinutes(AppointmentRules.DurationMinutes) <= schedule.EndTime)
                {
                    var slotEnd = slotStart.AddMinutes(AppointmentRules.DurationMinutes);

                    // Exclude slots that have already started or passed.
                    if (date == today && slotStart <= currentTime)
                    {
                        slotStart = slotEnd;
                        continue;
                    }

                    var hasConflict = blockingAppointments.Any(a =>
                    slotStart < a.EndTime &&
                    slotEnd > a.StartTime
                    );

                    if (!hasConflict)
                    {
                        availableSlots.Add(new AvailableAppointmentSlotDto
                        {
                            AppointmentDate = date,
                            StartTime = slotStart,
                            EndTime = slotEnd
                        });
                    }

                    slotStart = slotEnd;
                }

            }

            return availableSlots.DistinctBy(s => new { s.StartTime, s.EndTime, s.AppointmentDate })
                .OrderBy(s => s.StartTime)
                .ToList();

        }

        public async Task<IReadOnlyList<AppointmentResponseDto>> GetDoctorAppointmentsAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            if(doctorId == Guid.Empty)
            {
                throw new BadRequestException("Doctor ID cannot be empty.");
            }

            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<IReadOnlyList<AppointmentResponseDto>> GetMyAppointmentsAsync(Guid patientId, CancellationToken cancellationToken)
        {
            if(patientId == Guid.Empty)
            {
                throw new BadRequestException("Patient ID cannot be empty.");
            }

            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<AppointmentResponseDto> RejectAppointmentAsync(Guid appointmentId, Guid doctorId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (appointmentId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Appointment ID cannot be empty.");
            }

            if (doctorId == Guid.Empty)
            {
                throw new BadRequestException(
                    "Doctor ID cannot be empty.");
            }
            var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(
                appointmentId,
                cancellationToken
                );

            if (appointment is null)
            {
                throw new NotFoundException(
                    "Appointment not found.");
            }

            if(appointment.DoctorId != doctorId)
            {
                throw new UnauthorizedAccessException(
            "You are not authorized to reject this appointment.");
            }

            if (appointment.Status != AppointmentStatus.Pending)
            {
                throw new BadRequestException(
                    "Only pending appointments can be rejected.");
            }

            appointment.Status = AppointmentStatus.Rejected;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<AppointmentResponseDto> RescheduleAppointmentAsync(Guid appointmentId, 
            RescheduleAppointmentRequestDto request, Guid patientId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(request);

            if(appointmentId == Guid.Empty || patientId == Guid.Empty)
            {
                throw new BadRequestException("Appointment ID and patient ID are required.");
            }

            var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(
                appointmentId,
                cancellationToken
                );

            if(appointment is null)
            {
                throw new NotFoundException("Appointment not found.");
            }
            if(appointment.PatientId != patientId)
            {
                throw new UnauthorizedAccessException("You are not authorized to reschedule this appointment.");
            }
            if(appointment.Status != AppointmentStatus.Pending &&
                appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new BadRequestException("Only pending or confirmed appointments can be rescheduled.");
            }

            var endTime = await ValidateRequestedSlotAsync(
                appointment.DoctorId,
                request.AppointmentDate,
                request.StartTime,
                cancellationToken,
                appointment.Id
                );

            appointment.AppointmentDate = request.AppointmentDate;
            appointment.StartTime = request.StartTime;
            appointment.EndTime = endTime;
            appointment.Status = AppointmentStatus.Pending;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AppointmentResponseDto>(appointment);

        }

        private async Task<TimeOnly> ValidateRequestedSlotAsync(Guid doctorId,
            DateOnly date,
            TimeOnly startTime,
            CancellationToken cancellationToken,
            Guid? excludeAppointmentId = null
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var now = TimeZoneInfo.ConvertTime(
                        DateTimeOffset.UtcNow,
                        _clinicTimeZone);
            var today = DateOnly.FromDateTime(now.DateTime);
            var currentTime = TimeOnly.FromDateTime(now.DateTime);

            if (date < today)
            {
                throw new InvalidOperationException(
                    "Cannot book an appointment for a past date.");
            }
            if (date == today && startTime <= currentTime)
            {
                throw new InvalidOperationException(
                    "Cannot book a slot that has already started.");
            }

            var endTime = startTime.AddMinutes(AppointmentRules.DurationMinutes);

            //get the doctor schedule for selected date
             
            var schedules = await _doctorAvailabilityRepository.GetByDoctorAndDayAsync(doctorId,
                date,
                cancellationToken);

            //Verify the entire slot fits within a valid schedule.
            var fitsSchedule = schedules.Any(schedule =>
             schedule.IsAvailable &&
            startTime >= schedule.StartTime && 
            endTime <= schedule.EndTime &&
            (startTime - schedule.StartTime).TotalMinutes
                % AppointmentRules.DurationMinutes == 0);


            if (!fitsSchedule)
            {
                throw new InvalidOperationException("The selected slot is outside the doctor's schedule.");
            }

            //Check whether the doctor is unavailable on this date.

            var unavailablePeriods = await _doctorUnavailablePeriodRepository.GetOverlappingAsync(doctorId,
                date,
                cancellationToken
                );

            if (unavailablePeriods.Count > 0)
            {
                throw new InvalidOperationException("The doctor is unavailable on the selected date.");
            }

            //Recheck existing appointments.
            var appointments = await _appointmentRepository.GetByDoctorAndDateAsync(doctorId,
                date,
                cancellationToken);

            var hasConflict = appointments.Any(a =>
                a.Id != excludeAppointmentId &&
                (a.Status == AppointmentStatus.Pending ||
                a.Status == AppointmentStatus.Confirmed) &&
                startTime < a.EndTime &&
                endTime > a.StartTime);

            if (hasConflict) 
            {
                throw new BadRequestException(
                    "The selected slot has already been booked.");
            }

            return endTime;



        }

        
    }
}
