using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IReadOnlyList<Appointment>> GetByDoctorAndDateAsync(Guid doctorId,
            DateOnly date,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken);

        Task AddAsync(Appointment appointment, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
