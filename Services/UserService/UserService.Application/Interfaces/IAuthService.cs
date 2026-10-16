using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterPatientAsync(RegisterPatientRequest request , CancellationToken cancellationToken);
        Task RegisterDoctorAsync(RegisterDoctorRequest request , CancellationToken cancellationToken);
        Task RegisterLabAsync(RegisterLabRequest request , CancellationToken cancellationToken);

        Task<LoginResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken);

        Task<LoginResponse> LoginAsync(LoginRequest request , CancellationToken cancellationToken);
    }
}
