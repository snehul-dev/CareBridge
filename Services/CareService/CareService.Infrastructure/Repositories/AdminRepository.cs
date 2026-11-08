using CareBridge.Shared.Exceptions;
using CareService.Application.DTOs.Admin;
using CareService.Application.Interfaces;
using CareService.Domain.Enums;
using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace CareService.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly CareDbContext _context;
        public AdminRepository(CareDbContext context)
        {
            _context = context;
        }
        public async Task<ConsultationStatisticsDto> GetConsultationStatisticsAsync(
           DateTimeOffset? from,
           DateTimeOffset? to,
           CancellationToken cancellationToken)
        {
            // Convert incoming timestamps to UTC
            var fromUtc = from?.ToUniversalTime();
            var toUtc = to?.ToUniversalTime();

            var query = _context.Consultations
                .AsNoTracking()
                .AsQueryable();

            if (fromUtc.HasValue)
            {
                query = query.Where(
                    x => x.StartedAt >= fromUtc.Value);
            }

            if (toUtc.HasValue)
            {
                query = query.Where(
                    x => x.StartedAt <= toUtc.Value);
            }

            var groups = await query
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var counts = groups.ToDictionary(
                x => x.Status.ToString(),
                x => x.Count,
                StringComparer.OrdinalIgnoreCase);

            return new ConsultationStatisticsDto
            {
                TotalConsultations = counts.Values.Sum(),

                CompletedConsultations = GetCount(counts, "Completed"),

                ScheduledConsultations = GetCount(counts, "Scheduled"),

                CancelledConsultations = GetCount(counts, "Cancelled"),

                InProgressConsultations = GetCount(counts, "InProgress")
            };
        }

        public async Task<AdminDashboardResponseDto> GetDashboardAsync(
        CancellationToken cancellationToken)
        {
            var consultationGroups = await _context.Consultations
                .AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var labOrderGroups = await _context.LabOrders
                .AsNoTracking()
                .GroupBy(x => x.LabOrderStatus)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var consultationCounts = consultationGroups.ToDictionary(
                x => x.Status,
                x => x.Count);

            var labOrderCounts = labOrderGroups.ToDictionary(
                x => x.Status,
                x => x.Count);

            int GetConsultationCount(ConsultationStatus status)
                => consultationCounts.GetValueOrDefault(status, 0);

            int GetLabOrderCount(LabOrderStatus status)
                => labOrderCounts.GetValueOrDefault(status, 0);

            return new AdminDashboardResponseDto
            {
                TotalConsultations = consultationCounts.Values.Sum(),

                ScheduledConsultations = GetConsultationCount(
                    ConsultationStatus.Scheduled),

                InProgressConsultations = GetConsultationCount(
                    ConsultationStatus.InProgress),

                CompletedConsultations = GetConsultationCount(
                    ConsultationStatus.Completed),

                CancelledConsultations = GetConsultationCount(
                    ConsultationStatus.Cancelled),

                TotalLabOrders = labOrderCounts.Values.Sum(),

                PendingLabOrders =
                    GetLabOrderCount(LabOrderStatus.AwaitingLabSelection) +
                    GetLabOrderCount(LabOrderStatus.PendingAcceptance),

                CompletedLabOrders = GetLabOrderCount(
                    LabOrderStatus.Completed)
            };
        }

        public async Task<LabOrderStatisticsDto> GetLabOrderStatisticsAsync(
                string? status,
                CancellationToken cancellationToken)
        {
            var query = _context.LabOrders
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<LabOrderStatus>(
                    status.Trim(), true, out var parsedStatus)
                    || !Enum.IsDefined(parsedStatus))
                {
                    throw new BadRequestException(
                        "Invalid lab order status.");
                }

                query = query.Where(
                    x => x.LabOrderStatus == parsedStatus);
            }

            var groups = await query
                .GroupBy(x => x.LabOrderStatus)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var counts = groups.ToDictionary(
                x => x.Status.ToString(),
                x => x.Count,
                StringComparer.OrdinalIgnoreCase);

            return new LabOrderStatisticsDto
            {
                TotalLabOrders = counts.Values.Sum(),

                PendingLabOrders =
                    GetCount(counts, "AwaitingLabSelection") +
                    GetCount(counts, "PendingAcceptance"),

                AssignedLabOrders =
                    GetCount(counts, "Accepted") +
                    GetCount(counts, "SampleCollected") +
                    GetCount(counts, "InProgress"),

                CompletedLabOrders =
                    GetCount(counts, "Completed"),

                CancelledLabOrders =
                    GetCount(counts, "Cancelled"),

                RejectedLabOrders =
                    GetCount(counts, "Rejected")
            };
        }

        private static int GetCount(
            Dictionary<string, int> counts,
            string status)
        {
            return counts.TryGetValue(status, out var count)
                ? count
                : 0;
        }
    }
}
