using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<MedicalRecord?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<MedicalRecord?> GetByConsultationIdAsync(
            Guid consultationId,
            CancellationToken cancellationToken);

        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken);

        Task AddAsync(
            MedicalRecord medicalRecord,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
