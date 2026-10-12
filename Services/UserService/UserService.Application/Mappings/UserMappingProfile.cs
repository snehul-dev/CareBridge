
using AutoMapper;
using UserService.Application.DTOs.Auth;
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
        }
    }
}
