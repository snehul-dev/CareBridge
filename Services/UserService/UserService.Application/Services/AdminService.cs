using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using CareBridge.Shared.Exceptions;
using UserService.Application.DTOs.Admin;
using UserService.Application.Interfaces;
using UserService.Domain.Enums;

namespace UserService.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public AdminService(IAdminRepository adminRepository,
            IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            
        }

        public async Task ActivateUserAsync(Guid userId, Guid adminId,
            CancellationToken cancellationToken)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (userId == adminId)
                throw new BadRequestException(
                    "You cannot modify your own account.");

            if (user.IsActive)
                throw new BadRequestException(
                    "User account is already active.");

            user.IsActive = true;

            user.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken); 
        }

        public async Task ApproveDoctorAsync(Guid doctorId, Guid adminId, CancellationToken cancellationToken)
        {

            var doctor = await _adminRepository.GetDoctorByIdAsync(doctorId, cancellationToken);
            if(doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }

            if (doctor.VerificationStatus != VerificationStatus.Pending)
                throw new BadRequestException(
                    "Only pending doctor applications can be approved.");

            doctor.VerificationStatus = VerificationStatus.Approved;
            doctor.IsVerified = true;
            doctor.VerifiedAt = DateTime.UtcNow;
            doctor.VerifiedBy = adminId;
            doctor.RejectionReason = null;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken);

        }

        public async Task ApproveLabAsync(Guid labId, Guid adminId, CancellationToken cancellationToken)
        {
            var lab = await _adminRepository.GetLabByIdAsync(
                labId, cancellationToken);

            if (lab == null)
                throw new NotFoundException("Lab not found.");

            if (lab.VerificationStatus != VerificationStatus.Pending)
                throw new BadRequestException(
                    "Only pending lab applications can be approved.");

            lab.VerificationStatus = VerificationStatus.Approved;
            lab.IsVerified = true;
            lab.VerifiedAt = DateTime.UtcNow;
            lab.VerifiedBy = adminId;
            lab.RejectionReason = null;
            lab.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken);

        }

        public async Task DeactivateUserAsync(Guid userId, Guid adminId, CancellationToken cancellationToken)
        {
            var user = await _adminRepository.GetUserByIdAsync(
                userId,
                cancellationToken);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (userId == adminId)
                throw new BadRequestException(
                    "You cannot deactivate your own account.");

            if (!user.IsActive)
                throw new BadRequestException(
                    "User account is already inactive.");

            user.IsActive = false;

            user.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<AdminUserResponseDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _adminRepository.GetAllUsersAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AdminUserResponseDto>>(users);
        }

        public async Task<DoctorDetailsResponseDto> GetDoctorByIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            var doctor = await _adminRepository.GetDoctorByIdAsync(doctorId, cancellationToken);
            if(doctor == null)
            {
                throw new NotFoundException("Doctor not found");

            }

            return _mapper.Map<DoctorDetailsResponseDto>(doctor);
        }

        public async Task<LabDetailsResponseDto> GetLabByIdAsync(Guid labId, CancellationToken cancellationToken)
        {
            var lab = await _adminRepository.GetLabByIdAsync(
                labId, cancellationToken);

            if (lab == null)
                throw new NotFoundException("Lab not found.");

            return _mapper.Map<LabDetailsResponseDto>(lab);
        }

        public async Task<IEnumerable<PendingDoctorResponseDto>> GetPendingDoctorsAsync(CancellationToken cancellationToken)
        {
            var doctors = await _adminRepository.GetPendingDoctorsAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PendingDoctorResponseDto>>(doctors);
        }

        public async Task<IEnumerable<PendingLabResponseDto>> GetPendingLabsAsync(CancellationToken cancellationToken)
        {
            var labs = await _adminRepository.GetPendingLabsAsync(
                cancellationToken);

            return _mapper.Map<IEnumerable<PendingLabResponseDto>>(labs);
        }

        public async Task RejectDoctorAsync(Guid doctorId, Guid adminId, string reason, CancellationToken cancellationToken)
        {
            var doctor = await _adminRepository
               .GetDoctorByIdAsync(doctorId, cancellationToken);

            if (doctor == null)
                throw new NotFoundException("Doctor not found.");

            if (doctor.VerificationStatus != VerificationStatus.Pending)
                throw new BadRequestException(
                    "Only pending doctor applications can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(
                    "Rejection reason is required.", nameof(reason));

            doctor.VerificationStatus = VerificationStatus.Rejected;
            doctor.IsVerified = false;
            doctor.RejectionReason = reason.Trim();
            doctor.VerifiedAt = DateTime.UtcNow;
            doctor.VerifiedBy = adminId;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task RejectLabAsync(Guid labId, Guid adminId, string reason, CancellationToken cancellationToken)
        {
            var lab = await _adminRepository.GetLabByIdAsync(labId, cancellationToken);

            if(lab == null)
            {
                throw new NotFoundException("Lab not found");
            }

            if (lab.VerificationStatus != VerificationStatus.Pending)
                throw new BadRequestException(
                    "Only pending lab applications can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new BadRequestException(
                    "Rejection reason is required.");

            lab.VerificationStatus = VerificationStatus.Rejected;
            lab.IsVerified = false;
            lab.RejectionReason = reason.Trim();
            lab.VerifiedAt = DateTime.UtcNow;
            lab.VerifiedBy = adminId;
            lab.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(cancellationToken);


        }



    }
}
