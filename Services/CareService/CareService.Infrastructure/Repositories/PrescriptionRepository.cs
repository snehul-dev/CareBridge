using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly CareDbContext _context;
        public PrescriptionRepository(CareDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Prescription prescription, CancellationToken cancellationToken)
        {
            await _context.Prescriptions.AddAsync(prescription, cancellationToken);
        }

        public async Task<Prescription?> GetByConsultationIdAsync(Guid consultationId,
            CancellationToken cancellationToken)
        {
            return await _context.Prescriptions.
                AsNoTracking()
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.ConsultationId == consultationId, cancellationToken);
        }

        public async Task<Prescription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Items)
                 .FirstOrDefaultAsync(
                 p => p.Id == id,
                cancellationToken);
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Items)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
