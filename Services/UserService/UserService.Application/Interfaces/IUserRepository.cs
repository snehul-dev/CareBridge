using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email , CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id , CancellationToken cancellationToken);
        Task AddDoctorProfileAsync(DoctorProfile doctorProfile , CancellationToken cancellationToken);
        Task AddPatientProfileAsync(PatientProfile patientProfile , CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetMyProfileAsync(Guid userId , CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
