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
        public Task AddAsync(Consultation consultation)
        {
            throw new NotImplementedException();
        }

        public Task<Consultation?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Consultation>> GetByDoctorIdAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<Consultation?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Consultation consultation)
        {
            throw new NotImplementedException();
        }
    }
}
