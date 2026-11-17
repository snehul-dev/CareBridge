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

        Task<IReadOnlyList<Appointment>> GetActiveByDoctorAndDateRangeAsync(Guid doctorId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Appointment>> GetCancelledByUnavailablePeriodAsync(Guid unavailablePeriodId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken);

        Task AddAsync(Appointment appointment, CancellationToken cancellationToken);

        Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken);

        Task<Appointment?> GetAppointmentById(Guid id,Guid patientId, CancellationToken cancellationToken);

        Task<Appointment?> GetAppointmentForUpdateAsync(Guid id, CancellationToken cancellationToken);


        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
