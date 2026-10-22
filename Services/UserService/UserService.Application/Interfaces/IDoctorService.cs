using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Doctors;

namespace UserService.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<IReadOnlyList<DoctorSearchResponse>> SearchAsync(
            string? specialization,
            string? city,
            string? district,
            CancellationToken cancellationToken);

        Task<DoctorSearchResponse> GetByIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken);
    }
}
