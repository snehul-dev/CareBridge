using AppointmentService.Application.DTOs.Admin;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Enums;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentStatsRepository : IAppointmentStatsRepository
    {
        private static readonly string[] MonthNames =
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        private readonly AppointmentDbContext _context;

        public AppointmentStatsRepository(AppointmentDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken)
        {
            var groups = await _context.Appointments
                .AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var counts = groups.ToDictionary(
                x => x.Status,
                x => x.Count);

            int GetCount(AppointmentStatus status)
                => counts.GetValueOrDefault(status, 0);

            return new AppointmentStatisticsDto
            {
                TotalAppointments = counts.Values.Sum(),

                CompletedAppointments = GetCount(
                    AppointmentStatus.Completed),

                CancelledAppointments = GetCount(
                    AppointmentStatus.Cancelled),

                RescheduledAppointments = GetCount(
                    AppointmentStatus.Rescheduled),

                OngoingAppointments =
                    GetCount(AppointmentStatus.Pending) +
                    GetCount(AppointmentStatus.Confirmed)
            };
        }

        public async Task<IReadOnlyList<AppointmentTrendDto>> GetMonthlyTrendsAsync(
            int year,
            CancellationToken cancellationToken)
        {
            var startDate = new DateOnly(year, 1, 1);
            var endDate = new DateOnly(year, 12, 31);

            var groups = await _context.Appointments
                .AsNoTracking()
                .Where(x =>
                    x.AppointmentDate >= startDate &&
                    x.AppointmentDate <= endDate)
                .GroupBy(x => new
                {
                    x.AppointmentDate.Month,
                    x.Status
                })
                .Select(g => new
                {
                    g.Key.Month,
                    Status = g.Key.Status,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var trends = new List<AppointmentTrendDto>(12);

            for (var month = 1; month <= 12; month++)
            {
                var monthGroups = groups
                    .Where(x => x.Month == month)
                    .ToList();

                int GetCount(AppointmentStatus status)
                    => monthGroups
                        .Where(x => x.Status == status)
                        .Sum(x => x.Count);

                trends.Add(new AppointmentTrendDto
                {
                    Month = MonthNames[month - 1],

                    Completed = GetCount(AppointmentStatus.Completed),

                    Ongoing =
                        GetCount(AppointmentStatus.Pending) +
                        GetCount(AppointmentStatus.Confirmed),

                    Cancelled = GetCount(AppointmentStatus.Cancelled)
                });
            }

            return trends;
        }

        public async Task<IReadOnlyList<PopularDoctorAppointmentDto>> GetPopularDoctorsAsync(
            int limit,
            CancellationToken cancellationToken)
        {
            var rows = await _context.Appointments
                .AsNoTracking()
                .Where(x =>
                    x.Status != AppointmentStatus.Cancelled &&
                    x.Status != AppointmentStatus.Rejected)
                .GroupBy(x => x.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentCount)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return rows
                .Select(x => new PopularDoctorAppointmentDto
                {
                    DoctorId = x.DoctorId,
                    AppointmentCount = x.AppointmentCount
                })
                .ToList();
        }
    }
}
