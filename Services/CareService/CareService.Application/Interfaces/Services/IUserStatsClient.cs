using CareService.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces.Services
{
    public interface IUserStatsClient
    {
        Task<EntityCountsDto?> GetCountsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DoctorSummaryDto>?> GetDoctorSummariesAsync(
            IReadOnlyCollection<Guid> userIds,
            CancellationToken cancellationToken);
    }
}
