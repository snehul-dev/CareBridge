using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Users;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<MyProfileResponse> GetMyProfileAsync(Guid userId , CancellationToken cancellationToken);

        Task UpdateMyProfileAsync(Guid UserId, UpdateMyProfileRequest request , CancellationToken cancellationToken);

    }
}
