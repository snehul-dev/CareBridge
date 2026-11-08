
using AutoMapper;
using UserService.Application.DTOs.Admin;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.Doctors;
using UserService.Application.DTOs.Labs;
using UserService.Application.DTOs.Users;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // PatientRequest -> User
            CreateMap<RegisterPatientRequest, User>()
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.Gender)
                            ? (Gender?)null
                            : Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.PatientProfile, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorProfile, opt => opt.Ignore())
                .ForMember(dest => dest.LabProfile, opt => opt.Ignore())
                .ForMember(dest => dest.Addresses, opt => opt.Ignore());

            // DoctorRequest -> User
            CreateMap<RegisterDoctorRequest, User>()
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.Gender)
                            ? (Gender?)null
                            : Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.PatientProfile, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorProfile, opt => opt.Ignore())
                .ForMember(dest => dest.LabProfile, opt => opt.Ignore())
                .ForMember(dest => dest.Addresses, opt => opt.Ignore());

            // DoctorRequest -> DoctorProfile
            CreateMap<RegisterDoctorRequest, DoctorProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // LabRequest -> LabProfile
            CreateMap<RegisterLabRequest, LabProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // User to MyProfileResponse

            CreateMap<User, MyProfileResponse>();

            //DoctorProfile to DoctorSearchResponse

            CreateMap<DoctorProfile, DoctorSearchResponse>()
                .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.User.LastName))

                .ForMember(dest => dest.ProfileImageUrl,
                opt => opt.MapFrom(src => src.User.ProfileImageUrl))
                .ForMember(dest => dest.SpecializationName,
                opt => opt.MapFrom(src => src.Specialization.Name));

            //LabProfile to LabResponseDto

            CreateMap<LabProfile, LabResponseDto>();

            //DoctorProfile entity to dto

            CreateMap<DoctorProfile, PendingDoctorResponseDto>()
                .ForMember(dest => dest.DoctorId,
                opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))

                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User.Email))

                .ForMember(dest => dest.Specialization,
                opt => opt.MapFrom(src => src.Specialization.Name));

            //DoctorProfile entity to dto
            CreateMap<DoctorProfile, DoctorDetailsResponseDto>()
                .ForMember(
                    dest => dest.DoctorId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(
                    dest => dest.LastName,
                    opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(
                    dest => dest.Specialization,
                    opt => opt.MapFrom(src => src.Specialization.Name));

            CreateMap<LabProfile, PendingLabResponseDto>()
                .ForMember(d => d.LabId,
                    o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Email,
                    o => o.MapFrom(s => s.User.Email));

            CreateMap<LabProfile, LabDetailsResponseDto>()
                .ForMember(d => d.LabId,
                    o => o.MapFrom(s => s.Id))
                .ForMember(d => d.UserId,
                    o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.Email,
                    o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.PhoneNumber,
                    o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(d => d.VerificationStatus,
                    o => o.MapFrom(s => s.VerificationStatus.ToString()));

            CreateMap<User, AdminUserResponseDto>();
        }
    }
}
