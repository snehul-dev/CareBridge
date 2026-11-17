using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AutoMapper;
using CareBridge.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Services
{
    public class DoctorUnavailablePeriodService : IDoctorUnavailablePeriodService
    {
        private readonly IDoctorUnavailablePeriodRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public DoctorUnavailablePeriodService(
            IDoctorUnavailablePeriodRepository repository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<DoctorUnavailableResponseDto> CreateAsync(
            Guid doctorId,
            DoctorUnavailableRequestDto request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(request);

            if (doctorId == Guid.Empty)
            {
                throw new BadRequestException("Doctor ID cannot be empty.");
            }

            if (request.StartDate > request.EndDate)
            {
                throw new BadRequestException(
                    "Start date cannot be after end date.");
            }

            if (request.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new BadRequestException(
                    "Unavailable period cannot start in the past.");
            }

            if (!string.IsNullOrWhiteSpace(request.Reason) &&
                request.Reason.Length > 500)
            {
                throw new BadRequestException(
                    "Reason cannot exceed 500 characters.");
            }

            var hasOverlap = await _repository.HasOverlappingPeriodAsync(
                doctorId,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            if (hasOverlap)
            {
                throw new BadRequestException(
                    "An unavailable period already exists for the selected date range.");
            }

            var period = new DoctorUnavailablePeriod
            {
                Id = Guid.NewGuid(),
                DoctorId = doctorId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = string.IsNullOrWhiteSpace(request.Reason)
                    ? null
                    : request.Reason.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(period, cancellationToken);

            var affectedAppointments =
                await _appointmentRepository.GetActiveByDoctorAndDateRangeAsync(
                    doctorId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

            foreach (var appointment in affectedAppointments)
            {
                appointment.StatusBeforeUnavailable = appointment.Status;
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.UnavailablePeriodId = period.Id;
                appointment.UpdatedAt = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<DoctorUnavailableResponseDto>(period);
            response.CancelledAppointmentsCount = affectedAppointments.Count;
            return response;
        }

        public async Task<IReadOnlyList<DoctorUnavailableResponseDto>> GetMyUnavailablePeriodsAsync(
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (doctorId == Guid.Empty)
            {
                throw new BadRequestException("Doctor ID cannot be empty.");
            }

            var periods = await _repository.GetByDoctorAsync(
                doctorId,
                cancellationToken);

            return _mapper.Map<IReadOnlyList<DoctorUnavailableResponseDto>>(periods);
        }

        public async Task<int> DeleteAsync(
            Guid id,
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (id == Guid.Empty)
            {
                throw new BadRequestException("Unavailable period ID cannot be empty.");
            }

            if (doctorId == Guid.Empty)
            {
                throw new BadRequestException("Doctor ID cannot be empty.");
            }

            var period = await _repository.GetByIdAsync(id, cancellationToken);

            if (period is null)
            {
                throw new NotFoundException("Unavailable period not found.");
            }

            if (period.DoctorId != doctorId)
            {
                throw new ForbiddenException(
                    "You are not authorized to delete this unavailable period.");
            }

            var cancelledAppointments =
                await _appointmentRepository.GetCancelledByUnavailablePeriodAsync(
                    id,
                    cancellationToken);

            foreach (var appointment in cancelledAppointments)
            {
                appointment.Status =
                    appointment.StatusBeforeUnavailable ?? AppointmentStatus.Pending;
                appointment.StatusBeforeUnavailable = null;
                appointment.UnavailablePeriodId = null;
                appointment.UpdatedAt = DateTime.UtcNow;
            }

            _repository.Remove(period);
            await _repository.SaveChangesAsync(cancellationToken);

            return cancelledAppointments.Count;
        }
    }
}
