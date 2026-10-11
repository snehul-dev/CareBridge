using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Application.Mappings
{
    public class UserMappingProfile:Profile
    {
        public UserMappingProfile()
        {
            // PatientRequest to User
            CreateMap<RegisterPatientRequest, User>()
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.Gender)
                            ? (Gender?)null
                            : Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore());

            // DoctorRequest to User
            CreateMap<RegisterDoctorRequest, User>()
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.Gender)
                            ? (Gender?)null
                            : Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.Ignore());

            // DoctorRequest to DoctorProfile
            CreateMap<RegisterDoctorRequest, DoctorProfile>();
        }
    }
}
