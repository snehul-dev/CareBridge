using CareService.Application.DTOs.MedicalRecords;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordResponseDto> CreateAsync(
            CreateMedicalRecordRequestDto request,
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<IEnumerable<MedicalRecordResponseDto>> GetPatientHistoryAsync(
            Guid patientId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);

        Task<MedicalRecordResponseDto> GetByIdAsync(
            Guid recordId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);
    }
}
