using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly CareDbContext _context;

        public MedicalRecordRepository(CareDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken)
        {
            await _context.MedicalRecords.AddAsync(medicalRecord, cancellationToken);
        }

        public async Task<MedicalRecord?> GetByConsultationIdAsync(Guid consultationId, 
            CancellationToken cancellationToken)
        {
            return await _context.MedicalRecords.FirstOrDefaultAsync(x => 
            x.ConsultationId == consultationId,
             cancellationToken
            );
        }

        public async Task<MedicalRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.MedicalRecords.AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(Guid patientId, 
            CancellationToken cancellationToken)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
           await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
