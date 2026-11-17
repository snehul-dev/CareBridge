using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Repositories
{
    public class LabResultRepository : ILabResultRepository
    {
        private readonly CareDbContext _context;

        public LabResultRepository(CareDbContext context)
        {
            _context = context;
        }
        public async Task<LabResult?> GetByIdWithLabOrderAsync(Guid labResultId,
            CancellationToken cancellationToken)
        {
            return await _context.LabResults
                .Include(x => x.LabOrderItem)
                .ThenInclude(x => x.LabOrder)
                .FirstOrDefaultAsync(
                x => x.Id == labResultId,
                cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task UpdateAsync(LabResult labResult, CancellationToken cancellationToken)
        {
            _context.LabResults.Update(labResult);
            return Task.CompletedTask;
        }
    }
}
