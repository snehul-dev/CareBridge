using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IDoctorAvailabilityService
    {
        Task<IReadOnlyList<DoctorAvailabilityResponseDto>>AddWeeklyAvailabilityAsync(
            Guid doctorId,
            IReadOnlyList<DoctorAvailabilityRequestDto> requests,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DoctorAvailabilityResponseDto>>GetWeeklyAvailabilityAsync(
                Guid doctorId,
                CancellationToken cancellationToken);

        Task<DoctorAvailabilityResponseDto> UpdateAvailabilityAsync(
            Guid doctorId,
            Guid availabilityId,
            DoctorAvailabilityRequestDto request,
            CancellationToken cancellationToken);

        Task DeleteAvailabilityAsync(
            Guid doctorId,
            Guid availabilityId,
            CancellationToken cancellationToken);
    }
}
