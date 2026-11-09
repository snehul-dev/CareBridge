using System;
using System.Collections.Generic;
using System.Text;
using CareBridge.Contracts.Users;
using UserService.Application.DTOs.Users;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<MyProfileResponse> GetMyProfileAsync(Guid userId , CancellationToken cancellationToken);

        Task UpdateMyProfileAsync(Guid UserId, UpdateMyProfileRequest request ,
            CancellationToken cancellationToken);

   
        Task<PrescriptionProfileResponseDto>GetInternalPrescriptionProfileAsync(
        Guid doctorUserId,
        Guid patientUserId,
        CancellationToken cancellationToken);

        Task<UserPrescriptionProfileDto> GetPrescriptionProfileForServiceAsync(
            Guid userId,
            Guid currentUserId,
            string role,
            CancellationToken cancellationToken);


    }
}
