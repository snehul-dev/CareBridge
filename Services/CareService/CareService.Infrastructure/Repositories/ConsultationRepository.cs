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
        public  async Task AddAsync(Consultation consultation)
        {
            await _context.Consultations.AddAsync(consultation);
        }

        public async Task<Consultation?> GetByAppointmentIdAsync(Guid appointmentId)
        {
           return await _context.Consultations
                .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Consultation>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Consultations
                .Where(c => c.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<Consultation?> GetByIdAsync(Guid id)
        {
            return await _context.Consultations.FindAsync(id);
        }

        public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Consultations
                .Where(c => c.PatientId == patientId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Consultation consultation)
        {
             _context.Consultations.Update(consultation);
        }
    }
}
