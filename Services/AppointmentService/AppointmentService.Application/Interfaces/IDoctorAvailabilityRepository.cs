using AppointmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Application.Interfaces
{
    public interface IDoctorAvailabilityRepository
    {
        Task<IReadOnlyList<DoctorAvailability>> GetByDoctorAndDayAsync(Guid doctorId, DateOnly date,
            CancellationToken cancellationToken);
    }
}
