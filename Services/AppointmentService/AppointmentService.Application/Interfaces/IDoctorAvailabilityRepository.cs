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

        Task<IReadOnlyList<DoctorAvailability>> GetByDoctorAndDateRangeAsync(
            Guid doctorId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken);

        // Get a specific availability record belonging to a doctor
        Task<DoctorAvailability?> GetByIdAsync(
            Guid id,
            Guid doctorId,
            CancellationToken cancellationToken);

        // Add availability
        Task AddAsync(
            DoctorAvailability availability,
            CancellationToken cancellationToken);

        // Update availability
        void Update(DoctorAvailability availability);

        // Delete availability
        void Delete(DoctorAvailability availability);

        // Check overlapping availability
        Task<bool> HasOverlappingAvailabilityAsync(
            Guid doctorId,
            DateOnly date,
            TimeOnly startTime,
            TimeOnly endTime,
            Guid? excludeId,
            CancellationToken cancellationToken);

        // Persist changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
