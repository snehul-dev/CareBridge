using CareService.Application.DTOs.LabOrders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface ILabOrderService
    {
        // Doctor operations
        Task<LabOrderResponseDto> CreateAsync(
            CreateLabOrderRequestDto request,
            Guid doctorId,
            CancellationToken cancellationToken);

        Task<List<LabOrderResponseDto>> GetDoctorOrdersAsync(
            Guid doctorId,
            CancellationToken cancellationToken);

        // Patient operations
        Task<List<LabOrderResponseDto>> GetPatientOrdersAsync(
            Guid patientId,
            CancellationToken cancellationToken);

        Task<LabOrderResponseDto> SelectLabAsync(
            Guid labOrderId,
            SelectLabRequestDto request,
            Guid patientId,
            CancellationToken cancellationToken);

        // Lab operations
        Task<List<LabOrderResponseDto>> GetLabOrdersAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task<LabOrderResponseDto> AcceptOrderAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken);

        Task<LabOrderResponseDto> RejectOrderAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken);

        Task<LabOrderResponseDto> MarkSampleCollectedAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken);

        Task<LabOrderResponseDto> MarkInProgressAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken);

        Task<List<LabResultResponseDto>> SubmitResultsAsync(
            Guid labOrderId,
            SubmitLabResultsRequestDto request,
            Guid labId,
            CancellationToken cancellationToken);

        // Shared operations
        Task<LabOrderResponseDto> GetByIdAsync(
            Guid labOrderId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);

        Task<List<LabResultResponseDto>> GetResultsAsync(
            Guid labOrderId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);
    }
}
