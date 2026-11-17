using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IDoctorUnavailablePeriodRepository
    {
        Task<IReadOnlyList<DoctorUnavailablePeriod>> GetOverlappingAsync(Guid doctorId,
            DateOnly date,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DoctorUnavailablePeriod>> GetByDoctorAsync(Guid doctorId,
            CancellationToken cancellationToken);

        Task<DoctorUnavailablePeriod?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken);

        Task<bool> HasOverlappingPeriodAsync(Guid doctorId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken);

        Task AddAsync(DoctorUnavailablePeriod period,
            CancellationToken cancellationToken);

        void Remove(DoctorUnavailablePeriod period);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
