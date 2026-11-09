
using CareBridge.Contracts.Users;
using UserService.Application.DTOs.Users;
using CareBridge.Shared.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

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


        

        public async Task<PrescriptionProfileResponseDto>GetInternalPrescriptionProfileAsync(
        Guid doctorUserId,
        Guid patientUserId,
        CancellationToken cancellationToken)
        {
            var doctor = await _userRepository
                .GetPrescriptionProfileByIdAsync(
                    doctorUserId,
                    cancellationToken);

            if (doctor is null || doctor.DoctorProfile is null)
                throw new NotFoundException("Doctor profile not found.");

            var patient = await _userRepository
                .GetPrescriptionProfileByIdAsync(
                    patientUserId,
                    cancellationToken);

            if (patient is null || patient.PatientProfile is null)
                throw new NotFoundException("Patient profile not found.");

            return new PrescriptionProfileResponseDto
            {
                Doctor = new DoctorPrescriptionProfileDto
                {
                    UserId = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Qualification = doctor.DoctorProfile.Qualification,
                    HospitalOrClinicName =
                        doctor.DoctorProfile.HospitalOrClinicName,
                    DoctorAddress = doctor.DoctorProfile.Address,
                    City = doctor.DoctorProfile.City,
                    SpecializationName =
                        doctor.DoctorProfile.Specialization?.Name
                },

                Patient = new PatientPrescriptionProfileDto
                {
                    UserId = patient.Id,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Email = patient.Email,
                    PhoneNumber = patient.PhoneNumber,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender?.ToString(),
                    BloodGroup = patient.PatientProfile.BloodGroup
                }
            };
        }

        public async Task<UserPrescriptionProfileDto> GetPrescriptionProfileForServiceAsync(
            Guid userId,
            Guid currentUserId,
            string role,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetPrescriptionProfileByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            var isSelf = currentUserId == userId;
            var isDoctorAccessingPatient =
                role == "Doctor" && user.PatientProfile is not null;
            var isAdmin = role == "Admin";

            if (!isSelf && !isDoctorAccessingPatient && !isAdmin)
            {
                throw new ForbiddenException(
                    "You are not authorized to access this profile.");
            }

            return MapPrescriptionProfile(user);
        }

        private static UserPrescriptionProfileDto MapPrescriptionProfile(
            User user)
        {
            return new UserPrescriptionProfileDto
            {
                UserId = user.Id,

                FirstName = user.FirstName,
                LastName = user.LastName,

                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender?.ToString(),

                BloodGroup = user.PatientProfile?.BloodGroup,

                Qualification = user.DoctorProfile?.Qualification,

                HospitalOrClinicName =
                    user.DoctorProfile?.HospitalOrClinicName,

                DoctorAddress = user.DoctorProfile?.Address,

                City = user.DoctorProfile?.City,

                SpecializationName =
                    user.DoctorProfile?.Specialization?.Name
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
