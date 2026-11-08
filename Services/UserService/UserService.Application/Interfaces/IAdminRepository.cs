using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<DoctorProfile>> GetPendingDoctorsAsync(CancellationToken cancellationToken);

        Task<DoctorProfile?> GetDoctorByIdAsync(Guid doctorId, 
            CancellationToken cancellationToken);

        Task<IEnumerable<LabProfile>> GetPendingLabsAsync(
            CancellationToken cancellationToken);

        Task<LabProfile?> GetLabByIdAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task<IEnumerable<User>> GetAllUsersAsync(
            CancellationToken cancellationToken);

        Task<User?> GetUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

  

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
