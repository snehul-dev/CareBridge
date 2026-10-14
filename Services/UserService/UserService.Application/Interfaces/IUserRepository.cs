using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddDoctorProfileAsync(DoctorProfile doctorProfile);
        Task AddPatientProfileAsync(PatientProfile patientProfile);
        Task AddAsync(User user);
        Task<User?> GetMyProfileAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
