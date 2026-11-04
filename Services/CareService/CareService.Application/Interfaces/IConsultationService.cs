using CareService.Application.DTOs.Consultations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<ConsultationDetailsResponseDto> CreateAsync(CreateConsultationRequestDto request,
            Guid doctorId,
            CancellationToken cancellationToken
            );
        Task<ConsultationDetailsResponseDto> StartAsync(Guid consultationId, Guid doctorId , 
            CancellationToken cancellationToken
            );

        Task<ConsultationDetailsResponseDto> CompleteAsync(Guid consultationId, Guid doctorId,
            CancellationToken cancellationToken
            );

        Task<ConsultationDetailsResponseDto> GetByIdAsync(Guid consultationId, Guid userId, string role,
            CancellationToken cancellationToken);

        Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultationsForDoctorAsync(Guid patientId, Guid doctorId,
        CancellationToken cancellationToken);

        Task<IEnumerable<ConsultationDetailsResponseDto>> GetDoctorConsultationsAsync(Guid doctorId,
            CancellationToken cancellationToken);
        Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultaionAsync(Guid patientId ,
            CancellationToken cancellationToken);
    }
}
