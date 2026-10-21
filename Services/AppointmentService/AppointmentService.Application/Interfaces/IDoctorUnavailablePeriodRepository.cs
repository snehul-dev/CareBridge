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
    }
}
