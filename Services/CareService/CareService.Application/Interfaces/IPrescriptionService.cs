using CareService.Application.DTOs.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> CreateAsync(
            CreatePrescriptionRequestDto request,
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<PrescriptionResponseDto> GetByIdAsync(
            Guid prescriptionId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);

        Task<IEnumerable<PrescriptionResponseDto>> GetMyPrescriptionsAsync(
            Guid patientId,
            CancellationToken cancellationToken);

        Task<PrescriptionResponseDto> GetByConsultationIdAsync(
            Guid consultationId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);
    }
}
