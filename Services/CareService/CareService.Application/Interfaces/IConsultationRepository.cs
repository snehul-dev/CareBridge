using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IConsultationRepository
    {
        Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId);

        Task<IEnumerable<Consultation>> GetByDoctorIdAsync(Guid doctorId);

        Task<Consultation?> GetByAppointmentIdAsync(Guid appointmentId);

        Task<Consultation?> GetByIdAsync(Guid id);
        Task AddAsync(Consultation consultation);

        Task UpdateAsync(Consultation consultation);

        Task SaveChangesAsync();
    }
}
