using CareService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using CareService.Application.DTOs.Admin;
using CareBridge.Shared.Exceptions;

namespace CareService.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ConsultationStatisticsDto> GetConsultationStatisticsAsync(DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                throw new BadRequestException(
                    "From date cannot be greater than To date.");
            }

            return await _adminRepository
                .GetConsultationStatisticsAsync(
                    from,
                    to,
                    cancellationToken);
        }

        public async Task<AdminDashboardResponseDto> GetDashboardAsync(CancellationToken cancellationToken)
        {
            return await _adminRepository.GetDashboardAsync(
                cancellationToken);
        }

        public async Task<LabOrderStatisticsDto> GetLabOrderStatisticsAsync(string? status, CancellationToken cancellationToken)
        {
            return await _adminRepository.GetLabOrderStatisticsAsync(
               status,
               cancellationToken);
        }
    }
}

