
using AutoMapper;
using CareService.Application.DTOs.LabOrders;
using CareService.Application.Interfaces;
using CareService.Application.Interfaces.Services;
using CareService.Domain.Entities;
using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CareService.Application.Services
{
    public class LabOrderService : ILabOrderService
    {
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly IConsultationRepository _consultationRepository;
        private readonly ILabDirectoryClient _labDirectoryClient;
        private readonly IMapper _mapper;

        public LabOrderService(
            ILabOrderRepository labOrderRepository,
            IConsultationRepository consultationRepository,
            ILabDirectoryClient labDirectoryClient,
            IMapper mapper)
        {
            _labOrderRepository = labOrderRepository;
            _consultationRepository = consultationRepository;
            _labDirectoryClient = labDirectoryClient;
            _mapper = mapper;
        }

        public async Task<LabOrderResponseDto> CreateAsync(
            CreateLabOrderRequestDto request,
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            var consultation =
                await _consultationRepository.GetByIdAsync(
                    request.ConsultationId,
                    cancellationToken);

            if (consultation == null)
                throw new KeyNotFoundException("Consultation not found.");

            if (consultation.DoctorId != doctorId)
                throw new UnauthorizedAccessException(
                    "You cannot create a lab order for this consultation.");

            if (request.Items == null || !request.Items.Any())
                throw new InvalidOperationException(
                    "At least one test item is required.");

            var labOrder = _mapper.Map<LabOrder>(request);

            labOrder.Id = Guid.NewGuid();
            labOrder.ConsultationId = consultation.Id;
            labOrder.DoctorId = consultation.DoctorId;
            labOrder.PatientId = consultation.PatientId; 
            labOrder.AssignedLabId = null;
            labOrder.LabOrderStatus =
                LabOrderStatus.AwaitingLabSelection;
            labOrder.OrderedAt = DateTimeOffset.UtcNow;

            foreach (var item in labOrder.Items)
            {
                item.Id = Guid.NewGuid();
                item.CreatedAt = DateTimeOffset.UtcNow;
            }

            await _labOrderRepository.AddAsync(
                labOrder,
                cancellationToken);

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(labOrder);
        }

        //Get Doctor Laborders
        public async Task<List<LabOrderResponseDto>> GetDoctorOrdersAsync(
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            var orders =
                await _labOrderRepository.GetByDoctorIdAsync(
                    doctorId,
                    cancellationToken);

            return _mapper.Map<List<LabOrderResponseDto>>(orders);
        }

 
        //Get Patient laborders
        public async Task<List<LabOrderResponseDto>> GetPatientOrdersAsync(
            Guid patientId,
            CancellationToken cancellationToken)
        {
            var orders =
                await _labOrderRepository.GetByPatientIdAsync(
                    patientId,
                    cancellationToken);

            return _mapper.Map<List<LabOrderResponseDto>>(orders);
        }

      //select lab

        public async Task<LabOrderResponseDto> SelectLabAsync(
            Guid labOrderId,
            SelectLabRequestDto request,
            Guid patientId,
            CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(
                labOrderId,
                cancellationToken);

            if (order.PatientId != patientId)
                throw new UnauthorizedAccessException(
                    "You cannot select a lab for this order.");

            if (order.LabOrderStatus !=
                    LabOrderStatus.AwaitingLabSelection &&
                order.LabOrderStatus != LabOrderStatus.Rejected)
            {
                throw new InvalidOperationException(
                    "A lab cannot be selected for this order in its current status.");
            }

            var isVerified =
                await _labDirectoryClient.IsVerifiedLabAsync(
                    request.LabId,
                    cancellationToken);

            if (!isVerified)
                throw new InvalidOperationException(
                    "The selected lab does not exist or is not verified.");

            order.AssignedLabId = request.LabId;
            order.AssignedAt = DateTimeOffset.UtcNow;
            order.LabOrderStatus =
                LabOrderStatus.PendingAcceptance;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

           //Lab Get the Assigned Orders

        public async Task<List<LabOrderResponseDto>> GetLabOrdersAsync(
            Guid labId,
            CancellationToken cancellationToken)
        {
            var orders =
                await _labOrderRepository.GetByLabIdAsync(
                    labId,
                    cancellationToken);

            return _mapper.Map<List<LabOrderResponseDto>>(orders);
        }

        //Lab Accept Order

        public async Task<LabOrderResponseDto> AcceptOrderAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(
                labOrderId,
                cancellationToken);

            EnsureAssignedLab(order, labId);

            EnsureStatus(
                order,
                LabOrderStatus.PendingAcceptance);

            order.LabOrderStatus = LabOrderStatus.Accepted;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

      //Lab reject Order

        public async Task<LabOrderResponseDto> RejectOrderAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(
                labOrderId,
                cancellationToken);

            EnsureAssignedLab(order, labId);

            EnsureStatus(
                order,
                LabOrderStatus.PendingAcceptance);

            order.LabOrderStatus = LabOrderStatus.Rejected;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

        //Lab Sample collected
        public async Task<LabOrderResponseDto> MarkSampleCollectedAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(
                labOrderId,
                cancellationToken);

            EnsureAssignedLab(order, labId);

            EnsureStatus(
                order,
                LabOrderStatus.Accepted);

            order.LabOrderStatus =
                LabOrderStatus.SampleCollected;

            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

        //Lab marks progress

        public async Task<LabOrderResponseDto> MarkInProgressAsync(
            Guid labOrderId,
            Guid labId,
            CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(
                labOrderId,
                cancellationToken);

            EnsureAssignedLab(order, labId);

            EnsureStatus(
                order,
                LabOrderStatus.SampleCollected);

            order.LabOrderStatus = LabOrderStatus.InProgress;
            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

     //Lab submit result

        public async Task<List<LabResultResponseDto>> SubmitResultsAsync(
            Guid labOrderId,
            SubmitLabResultsRequestDto request,
            Guid labId,
            CancellationToken cancellationToken)
        {
            var order =
                await _labOrderRepository.GetByIdWithResultsAsync(
                    labOrderId,
                    cancellationToken);

            if (order == null)
                throw new KeyNotFoundException(
                    "Lab order not found.");

            EnsureAssignedLab(order, labId);

            EnsureStatus(
                order,
                LabOrderStatus.InProgress);

            if (request.Results == null ||
                !request.Results.Any())
            {
                throw new InvalidOperationException(
                    "At least one test result is required.");
            }

            foreach (var resultRequest in request.Results)
            {
                var item = order.Items.FirstOrDefault(
                    x => x.Id == resultRequest.LabOrderItemId);

                if (item == null)
                {
                    throw new InvalidOperationException(
                        $"Test item {resultRequest.LabOrderItemId} does not belong to this order.");
                }

                if (item.LabResults.Any())
                {
                    throw new InvalidOperationException(
                        $"A result has already been submitted for {item.TestName}.");
                }

                var result = new LabResult
                {
                    Id = Guid.NewGuid(),
                    LabOrderItemId = item.Id,
                    ResultValue = resultRequest.ResultValue,
                    Unit = resultRequest.Unit,
                    ReferenceRange = resultRequest.ReferenceRange,
                    Interpretation = resultRequest.Interpretation,
                    Notes = resultRequest.Notes,
                    ReportFileUrl = resultRequest.ReportFileUrl,
                    ResultDate = DateTimeOffset.UtcNow,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _labOrderRepository.AddLabResultAsync(
                         result,
                        cancellationToken);
            }

            var allResultsSubmitted = order.Items.All(
                item => item.LabResults.Any());

            if (allResultsSubmitted)
            {
                order.LabOrderStatus = LabOrderStatus.Completed;
                order.CompletedAt = DateTimeOffset.UtcNow;
            }

            order.UpdatedAt = DateTimeOffset.UtcNow;

            await _labOrderRepository.SaveChangesAsync(
                cancellationToken);

            return order.Items
                .SelectMany(item => item.LabResults.Select(result =>
                {
                    var dto =
                        _mapper.Map<LabResultResponseDto>(result);

                    dto.TestName = item.TestName;

                    return dto;
                }))
                .ToList();
        }

       //Get order By Id

        public async Task<LabOrderResponseDto> GetByIdAsync(
            Guid labOrderId,
            Guid userId,
            string role,
            CancellationToken cancellationToken)
        {
            var order =
                await _labOrderRepository.GetByIdWithItemsAsync(
                    labOrderId,
                    cancellationToken);

            if (order == null)
                throw new KeyNotFoundException(
                    "Lab order not found.");

            EnsureOrderAccess(order, userId, role);

            return _mapper.Map<LabOrderResponseDto>(order);
        }

       //Get results

        public async Task<List<LabResultResponseDto>> GetResultsAsync(
            Guid labOrderId,
            Guid userId,
            string role,
            CancellationToken cancellationToken)
        {
            var order =
                await _labOrderRepository.GetByIdWithResultsAsync(
                    labOrderId,
                    cancellationToken);

            if (order == null)
                throw new KeyNotFoundException(
                    "Lab order not found.");

            EnsureOrderAccess(order, userId, role);

            return order.Items
                .SelectMany(item => item.LabResults.Select(result =>
                {
                    var dto =
                        _mapper.Map<LabResultResponseDto>(result);

                    dto.TestName = item.TestName;

                    return dto;
                }))
                .ToList();
        }

        

        private async Task<LabOrder> GetOrderWithItems(
            Guid id,
            CancellationToken cancellationToken)
        {
            var order =
                await _labOrderRepository.GetByIdWithItemsAsync(
                    id,
                    cancellationToken);

            return order ?? throw new KeyNotFoundException(
                "Lab order not found.");
        }

        private static void EnsureAssignedLab(
            LabOrder order,
            Guid labId)
        {
            if (order.AssignedLabId != labId)
            {
                throw new UnauthorizedAccessException(
                    "This order is not assigned to your lab.");
            }
        }

        private static void EnsureStatus(
            LabOrder order,
            LabOrderStatus expectedStatus)
        {
            if (order.LabOrderStatus != expectedStatus)
            {
                throw new InvalidOperationException(
                    $"This operation requires status {expectedStatus}. " +
                    $"Current status: {order.LabOrderStatus}.");
            }
        }

        private static void EnsureOrderAccess(
            LabOrder order,
            Guid userId,
            string role)
        {
            var hasAccess = role.ToLowerInvariant() switch
            {
                "doctor" => order.DoctorId == userId,
                "patient" => order.PatientId == userId,
                "lab" => order.AssignedLabId == userId,
                _ => false
            };

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to access this lab order.");
            }
        }
    }
}
