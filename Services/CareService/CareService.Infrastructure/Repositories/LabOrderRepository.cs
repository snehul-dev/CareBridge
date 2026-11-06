using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Repositories
{
    public class LabOrderRepository : ILabOrderRepository
    {

        private readonly CareDbContext _context;
        public LabOrderRepository(CareDbContext context)
        {
            _context = context;
        }
        public async Task<LabOrder?> GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<LabOrder?> GetByIdWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<LabOrder?> GetByIdWithResultsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .Include(x => x.Items)
                    .ThenInclude(x => x.LabResults)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<List<LabOrder>> GetByDoctorIdAsync(
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .Include(x => x.Items)
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.OrderedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LabOrder>> GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .Include(x => x.Items)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.OrderedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LabOrder>> GetByLabIdAsync(
            Guid labId,
            CancellationToken cancellationToken)
        {
            return await _context.LabOrders
                .Include(x => x.Items)
                .Where(x => x.AssignedLabId == labId)
                .OrderByDescending(x => x.OrderedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            LabOrder labOrder,
            CancellationToken cancellationToken)
        {
            await _context.LabOrders.AddAsync(
                labOrder,
                cancellationToken);
        }

        public Task UpdateAsync(
            LabOrder labOrder,
            CancellationToken cancellationToken)
        {
            _context.LabOrders.Update(labOrder);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
