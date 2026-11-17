using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface ILabResultRepository
    {
        Task<LabResult?> GetByIdWithLabOrderAsync(
            Guid labResultId,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            LabResult labResult,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
