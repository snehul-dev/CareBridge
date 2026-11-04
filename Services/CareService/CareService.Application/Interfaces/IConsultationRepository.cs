using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IConsultationRepository
    {
        Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId , CancellationToken cancellationToken);

        Task<IEnumerable<Consultation>> GetByDoctorIdAsync(Guid doctorId , CancellationToken cancellationToken);

        Task<Consultation?> GetByAppointmentIdAsync(Guid appointmentId,CancellationToken cancellationToken);
        Task<IEnumerable<Consultation>> GetByPatientAndDoctorAsync(
            Guid patientId,
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<Consultation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Consultation consultation,CancellationToken cancellationToken);

        void Update(Consultation consultation);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
