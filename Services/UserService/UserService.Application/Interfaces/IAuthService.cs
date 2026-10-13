using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterPatientAsync(RegisterPatientRequest request);
        Task RegisterDoctorAsync(RegisterDoctorRequest request);
        Task RegisterLabAsync(RegisterLabRequest request);
      

        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
