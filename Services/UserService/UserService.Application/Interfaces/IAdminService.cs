using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Admin;

namespace UserService.Application.Interfaces
{
    public interface IAdminService
    {

        //Doctor
        Task<IEnumerable<PendingDoctorResponseDto>> GetPendingDoctorsAsync(
            CancellationToken cancellationToken);

        Task<DoctorDetailsResponseDto> GetDoctorByIdAsync(
            Guid doctorId, CancellationToken cancellationToken);

        Task ApproveDoctorAsync(
            Guid doctorId,
            Guid adminId,
       CancellationToken cancellationToken);

        Task RejectDoctorAsync(
            Guid doctorId,
            Guid adminId,
            string reason,
            CancellationToken cancellationToken);



        //Lab

        Task<IEnumerable<PendingLabResponseDto>> GetPendingLabsAsync(
            CancellationToken cancellationToken);

        Task<LabDetailsResponseDto> GetLabByIdAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task ApproveLabAsync(
            Guid labId,
            Guid adminId,
            CancellationToken cancellationToken);

        Task RejectLabAsync(
            Guid labId,
            Guid adminId,
            string reason,
            CancellationToken cancellationToken);

        //user

        Task<IEnumerable<AdminUserResponseDto>> GetAllUsersAsync(
            CancellationToken cancellationToken);

        Task ActivateUserAsync(
            Guid userId,
            Guid adminId,
            CancellationToken cancellationToken);

        Task DeactivateUserAsync(
            Guid userId,
            Guid adminId,
            CancellationToken cancellationToken);



    }
}
