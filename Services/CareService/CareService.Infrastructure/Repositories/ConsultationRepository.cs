using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Repositories
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly CareDbContext _context;
        public ConsultationRepository(CareDbContext context)
        {
            _context = context;
        }
        public  async Task AddAsync(Consultation consultation , CancellationToken cancellationToken)
        {
            await _context.Consultations.AddAsync(consultation , cancellationToken);
        }

        public async Task<Consultation?> GetByAppointmentIdAsync(Guid appointmentId, 
            CancellationToken cancellationToken)
        {
           return await _context.Consultations
                .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId , cancellationToken);
        }

        public async Task<IEnumerable<Consultation>> GetByDoctorIdAsync(Guid doctorId, 
            CancellationToken cancellationToken)
        {
            return await _context.Consultations
                .AsNoTracking()
                .Where(c => c.DoctorId == doctorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Consultation?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Consultations
                .FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Consultation>> GetByPatientAndDoctorAsync(Guid patientId, 
            Guid doctorId, CancellationToken cancellationToken)
        {
            return await _context.Consultations
                .AsNoTracking()
                .Where(c => c.PatientId == patientId && 
                c.DoctorId == doctorId)
                   .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId  ,
            CancellationToken cancellationToken)
        {
            return await _context.Consultations
                .AsNoTracking()
                .Where(c => c.PatientId == patientId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Consultation consultation)
        {
             _context.Consultations.Update(consultation );
        }

        
    }
}
