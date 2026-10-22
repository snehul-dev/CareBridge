using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IReadOnlyList<DoctorProfile>> SearchAsync(
            string? specialization,
            string? city,
            string? district,
            CancellationToken cancellationToken);

        Task<DoctorProfile?> GetVerifiedByIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken);
    }
}
