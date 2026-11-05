using CareService.Application.DTOs.Prescriptions;
using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription?> GetByIdAsync(
         Guid id,
         CancellationToken cancellationToken);

        Task<Prescription?> GetByConsultationIdAsync(
            Guid consultationId,
            CancellationToken cancellationToken);

        Task<IEnumerable<Prescription>> GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken);

        Task AddAsync(
            Prescription prescription,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);



    }
}
