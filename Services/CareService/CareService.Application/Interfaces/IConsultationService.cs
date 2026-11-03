using CareService.Application.DTOs.Consultations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<ConsultationDetailsResponseDto> CreateAsync(CreateConsultationRequestDto request,
            Guid doctorId
            );
        Task<ConsultationDetailsResponseDto> StartAsync(Guid consultationId, Guid doctorId);

        Task<ConsultationDetailsResponseDto> CompleteAsync(Guid consultationId, Guid doctorId);

        Task<ConsultationDetailsResponseDto> GeyByIdAsync(Guid consultationId, Guid userId, string role);

        Task<IEnumerable<ConsultationDetailsResponseDto>> GetDoctorConsultationsAsync(Guid doctorId);
        Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultaionAsync(Guid patientId);
    }
}
