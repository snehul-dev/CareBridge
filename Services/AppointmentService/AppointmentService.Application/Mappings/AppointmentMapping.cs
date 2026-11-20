using AppointmentService.Application.DTOs;
using AppointmentService.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Mappings
{
    public class AppointmentMapping:Profile
    {
        public AppointmentMapping()
        {
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(
                    d => d.AppointmentType,
                    o => o.MapFrom(s => s.AppointmentType.ToString()));

            CreateMap<DoctorUnavailablePeriod, DoctorUnavailableResponseDto>();
        }
    }
}
