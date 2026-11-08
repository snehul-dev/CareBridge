using AutoMapper;
using CareService.Application.DTOs.Consultations;
using CareService.Application.DTOs.LabOrders;
using CareService.Application.DTOs.MedicalRecords;
using CareService.Application.DTOs.Prescriptions;
using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Mappings
{
    public  class CareMappingProfile : Profile
    {
        public CareMappingProfile()
        {
            //consultation to dto

            CreateMap<Consultation, ConsultationDetailsResponseDto>();

            //medicalrecordrequestdto to medicalrecord

            CreateMap<CreateMedicalRecordRequestDto,MedicalRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.ConsultationId, opt => opt.Ignore())
                .ForMember(dest => dest.Consultation, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            //Medicalrecord to medicalresponseDto

            CreateMap<MedicalRecord, MedicalRecordResponseDto>();

            //prescriptionRequestDto to prescription

            CreateMap<CreatePrescriptionRequestDto, Prescription>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.Consultation, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PdfFileUrl, opt => opt.Ignore());

            //prescriptionItemRequestDto to prescription

            CreateMap<PrescriptionItemRequestDto, PrescriptionItem>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.PrescriptionId, opt => opt.Ignore())
               .ForMember(dest => dest.Prescription, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            //entity to dto
            CreateMap<Prescription, PrescriptionResponseDto>();

            CreateMap<PrescriptionItem, PrescriptionItemResponseDto>();

            // laborder to laborderResponsedto 

            // Request DTO -> Entity
            CreateMap<CreateLabOrderRequestDto, LabOrder>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ConsultationId, o => o.Ignore())
                .ForMember(d => d.DoctorId, o => o.Ignore())
                .ForMember(d => d.PatientId, o => o.Ignore())
                .ForMember(d => d.AssignedLabId, o => o.Ignore())
                .ForMember(d => d.LabOrderStatus, o => o.Ignore())
                .ForMember(d => d.OrderedAt, o => o.Ignore())
                .ForMember(d => d.AssignedAt, o => o.Ignore())
                .ForMember(d => d.CompletedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.Consultation, o => o.Ignore());

            // Lab Order Item Request DTO -> Entity
            CreateMap<LabOrderItemRequestDto, LabOrderItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.LabOrderId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());

            // Entity -> Response DTO
            CreateMap<LabOrder, LabOrderResponseDto>()
                .ForMember(d => d.LabOrderStatus,
                    o => o.MapFrom(s => s.LabOrderStatus.ToString()));

            CreateMap<LabOrderItem, LabOrderItemResponseDto>();

            CreateMap<LabResult, LabResultResponseDto>()
                .ForMember(d => d.TestName, o => o.Ignore());

        }
    } 
}
