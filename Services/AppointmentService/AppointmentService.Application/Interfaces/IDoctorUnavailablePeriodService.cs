using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IDoctorUnavailablePeriodService
    {
        Task<DoctorUnavailableResponseDto> CreateAsync(
            Guid doctorId,
            DoctorUnavailableRequestDto request,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DoctorUnavailableResponseDto>> GetMyUnavailablePeriodsAsync(
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<int> DeleteAsync(
            Guid id,
            Guid doctorId,
            CancellationToken cancellationToken);
    }
}
