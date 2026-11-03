using CareService.Application.DTOs.Consultations;
using CareService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        public Task<ConsultationDetailsResponseDto> CompleteAsync(Guid consultationId, Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultationDetailsResponseDto> CreateAsync(CreateConsultationRequestDto request, Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ConsultationDetailsResponseDto>> GetDoctorConsultationsAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultaionAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultationDetailsResponseDto> GeyByIdAsync(Guid consultationId, Guid userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultationDetailsResponseDto> StartAsync(Guid consultationId, Guid doctorId)
        {
            throw new NotImplementedException();
        }
    }
}
