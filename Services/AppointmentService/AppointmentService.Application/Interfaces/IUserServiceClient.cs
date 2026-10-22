using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<IReadOnlyList<DoctorSearchResponse>> SearchDoctorsAsync(
            string? specialization,
            string? city,
            string? district,
            CancellationToken cancellationToken);

        Task<DoctorSearchResponse?> GetDoctorByIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken);
    }
}
