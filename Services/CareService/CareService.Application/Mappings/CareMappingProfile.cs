using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CareService.Application.DTOs.Consultations;
using CareService.Domain.Entities;

namespace CareService.Application.Mappings
{
    public  class CareMappingProfile : Profile
    {
        public CareMappingProfile()
        {
            //consultation to dto
            CreateMap<Consultation, ConsultationDetailsResponseDto>();
        }
    } 
}
