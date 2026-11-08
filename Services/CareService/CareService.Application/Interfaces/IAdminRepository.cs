using System;
using System.Collections.Generic;
using System.Text;
using CareService.Application.DTOs.Admin;

namespace CareService.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<AdminDashboardResponseDto> GetDashboardAsync(
           CancellationToken cancellationToken);

        Task<ConsultationStatisticsDto> GetConsultationStatisticsAsync(
            DateTimeOffset? from,
            DateTimeOffset? to,
            CancellationToken cancellationToken);

        Task<LabOrderStatisticsDto> GetLabOrderStatisticsAsync(
            string? status,
            CancellationToken cancellationToken);
    }
}
