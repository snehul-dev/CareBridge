using AppointmentService.Application.DTOs;
using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentService
    {

        Task<IReadOnlyList<AvailableAppointmentSlotDto>> GetAvailableSlotsAsync(Guid doctorId,
            DateOnly date,
            CancellationToken cancellationToken
            );

        Task<AppointmentResponseDto> BookAppointmentAsync(BookAppointmentRequestDto request,
            Guid patientId,
            CancellationToken cancellationToken
            );
        Task<IReadOnlyList<AppointmentResponseDto>> GetMyAppointmentsAsync(Guid patientId, 
            CancellationToken cancellationToken);

        Task<IReadOnlyList<AppointmentResponseDto>> GetDoctorAppointmentsAsync(
            Guid doctorId, CancellationToken cancellationToken
            );

        Task<AppointmentResponseDto> GetAppointmentById(Guid id, Guid patientId, CancellationToken cancellationToken);
    }
}
