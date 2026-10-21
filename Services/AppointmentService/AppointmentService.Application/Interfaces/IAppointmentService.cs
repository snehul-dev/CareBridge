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
    }
}
