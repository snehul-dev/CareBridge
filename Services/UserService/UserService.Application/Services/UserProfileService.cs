
using UserService.Application.DTOs.Users;
using CareBridge.Shared.Exceptions;
using UserService.Application.Interfaces;

namespace UserService.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;

        public UserProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        
        public async Task<MyProfileResponse> GetMyProfileAsync(Guid userId , 
            CancellationToken cancellationToken
            )
        {
            var user = await _userRepository.GetByIdAsync(userId , cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            return new MyProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
               

                PatientProfile = user.PatientProfile is null
                    ? null
                    : new PatientProfileResponse
                    {
                        BloodGroup = user.PatientProfile.BloodGroup,
                        EmergencyContactName =
                            user.PatientProfile.EmergencyContactName,
                        MedicalHistory =
                            user.PatientProfile.MedicalHistory
                    },

                DoctorProfile = user.DoctorProfile is null
                    ? null
                    : new DoctorProfileResponse
                    {
                        SpecializationId =
                            user.DoctorProfile.SpecializationId,
                        LicenseNumber =
                            user.DoctorProfile.LicenseNumber,
                        Qualification =
                            user.DoctorProfile.Qualification,
                        ExperienceYears =
                            user.DoctorProfile.ExperienceYears,
                        ConsultationFee =
                            user.DoctorProfile.ConsultationFee,
                        HospitalOrClinicName =
                            user.DoctorProfile.HospitalOrClinicName,
                        City = user.DoctorProfile.City,
                        District = user.DoctorProfile.District,
                        Bio = user.DoctorProfile.Bio,
                        IsVerified = user.DoctorProfile.IsVerified
                    },

                LabProfile = user.LabProfile is null
                    ? null
                    : new LabProfileResponse
                    {
                        LabName = user.LabProfile.LabName,
                        LicenseNumber = user.LabProfile.LicenseNumber,
                        Description = user.LabProfile.Description,
                        AddressLine = user.LabProfile.AddressLine,
                        City = user.LabProfile.City,
                        District = user.LabProfile.District,
                        State = user.LabProfile.State,
                        Bio = user.LabProfile.Bio,
                        IsVerified = user.LabProfile.IsVerified
                    }
            };
        }

        public async Task UpdateMyProfileAsync( Guid userId,UpdateMyProfileRequest request ,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId , cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

           //common user
            if (request.FirstName is not null)
                user.FirstName = request.FirstName.Trim();

            if (request.LastName is not null)
                user.LastName = request.LastName.Trim();

            if (request.PhoneNumber is not null)
                user.PhoneNumber = request.PhoneNumber.Trim();

            if (request.Gender.HasValue)
                user.Gender = request.Gender;

            if (request.DateOfBirth.HasValue)
            {
                if (request.DateOfBirth.Value.Date > DateTime.UtcNow.Date)
                    throw new BadRequestException(
                        "Date of birth cannot be in the future.");

                user.DateOfBirth = request.DateOfBirth;
            }

            user.UpdatedAt = DateTime.UtcNow;

            // Patient profile
            if (user.PatientProfile is not null)
            {
                if (request.BloodGroup is not null)
                    user.PatientProfile.BloodGroup =
                        request.BloodGroup.Trim();

                if (request.EmergencyContactName is not null)
                    user.PatientProfile.EmergencyContactName =
                        request.EmergencyContactName.Trim();

                if (request.MedicalHistory is not null)
                    user.PatientProfile.MedicalHistory =
                        request.MedicalHistory.Trim();

                user.PatientProfile.UpdatedAt = DateTime.UtcNow;
            }

            // Doctor profile
            if (user.DoctorProfile is not null)
            {
                if (request.ExperienceYears.HasValue)
                {
                    if (request.ExperienceYears.Value < 0)
                        throw new BadRequestException(
                            "Experience years cannot be negative.");

                    user.DoctorProfile.ExperienceYears =
                        request.ExperienceYears.Value;
                }

                if (request.ConsultationFee.HasValue)
                {
                    if (request.ConsultationFee.Value < 0)
                        throw new BadRequestException(
                            "Consultation fee cannot be negative.");

                    user.DoctorProfile.ConsultationFee =
                        request.ConsultationFee.Value;
                }

                if (request.HospitalOrClinicName is not null)
                    user.DoctorProfile.HospitalOrClinicName =
                        request.HospitalOrClinicName.Trim();

                if (request.City is not null)
                    user.DoctorProfile.City = request.City.Trim();

                if (request.District is not null)
                    user.DoctorProfile.District =
                        request.District.Trim();

                if (request.DoctorBio is not null)
                    user.DoctorProfile.Bio = request.DoctorBio.Trim();

                user.DoctorProfile.UpdatedAt = DateTime.UtcNow;
            }

            // Lab profile
            if (user.LabProfile is not null)
            {
                if (request.LabName is not null)
                    user.LabProfile.LabName = request.LabName.Trim();

                if (request.Description is not null)
                    user.LabProfile.Description =
                        request.Description.Trim();

                if (request.AddressLine is not null)
                    user.LabProfile.AddressLine =
                        request.AddressLine.Trim();

                if (request.LabCity is not null)
                    user.LabProfile.City = request.LabCity.Trim();

                if (request.LabDistrict is not null)
                    user.LabProfile.District =
                        request.LabDistrict.Trim();

                if (request.State is not null)
                    user.LabProfile.State = request.State.Trim();

                if (request.LabBio is not null)
                    user.LabProfile.Bio = request.LabBio.Trim();

                user.LabProfile.UpdatedAt = DateTime.UtcNow;
            }

            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
