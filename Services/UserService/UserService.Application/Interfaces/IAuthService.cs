using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;

namespace UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterPatientAsync(RegisterPatientRequest request);
        Task RegisterDoctorAsync(RegisterDoctorRequest request);
    }
}
