using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface ILabOrderRepository
    {
        Task<LabOrder?> GetByIdAsync(
           Guid id,
           CancellationToken cancellationToken);

        Task<LabOrder?> GetByIdWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<LabOrder?> GetByIdWithResultsAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<List<LabOrder>> GetByDoctorIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<List<LabOrder>> GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken);

        Task<List<LabOrder>> GetByLabIdAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task AddAsync(
            LabOrder labOrder,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            LabOrder labOrder,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
