using AutoMapper;
using CareBridge.Shared.Exceptions;
using CareService.Application.DTOs.Prescriptions;
using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using CareBridge.Contracts.Messaging;


namespace CareService.Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IUserProfileClient _userProfileClient;
        private readonly IMapper _mapper;
        private readonly ILogger<PrescriptionService> _logger;
        public PrescriptionService(IPrescriptionRepository prescriptionRepository , 
            IConsultationRepository consultationRepository,
            IMessagePublisher messagePublisher,
            IUserProfileClient userProfileClient,
            IMapper mapper,
            ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository;
            _consultationRepository = consultationRepository;
            _messagePublisher = messagePublisher;
            _userProfileClient = userProfileClient;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PrescriptionResponseDto> CreateAsync(CreatePrescriptionRequestDto request,
            Guid doctorId, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
            {
                throw new BadRequestException(
                    "At least one medicine is required.");
            }

            if (request.Items.Any(item =>
            string.IsNullOrWhiteSpace(item.MedicineName) ||
            string.IsNullOrWhiteSpace(item.Dosage) ||
            string.IsNullOrWhiteSpace(item.Frequency) ||
            string.IsNullOrWhiteSpace(item.Duration)))
            {
                throw new BadRequestException(
                    "Medicine name, dosage, frequency and duration are required.");
            }

            if (string.IsNullOrWhiteSpace(request.Diagnosis))
            {
                throw new BadRequestException("Diagnosis is required.");
            }

            if (request.Diagnosis.Trim().Length > 2000 ||
                (request.TreatmentPlan?.Trim().Length ?? 0) > 3000 ||
                (request.ClinicalNotes?.Trim().Length ?? 0) > 5000)
            {
                throw new BadRequestException(
                    "Diagnosis must be 2000 characters or fewer, treatment plan 3000 or fewer, and clinical notes 5000 or fewer.");
            }

            var consultation = await _consultationRepository.GetByIdAsync(request.ConsultationId,
                cancellationToken);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            if (consultation.DoctorId != doctorId)
            {
                throw new ForbiddenException(
                    "You are not authorized to create a prescription for this consultation.");
            }

            if (consultation.Status != ConsultationStatus.InProgress &&
                consultation.Status != ConsultationStatus.Completed)
            {
                throw new BadRequestException(
                    "Prescriptions can only be created for ongoing or completed consultations.");
            }

            var existingPrescription = await _prescriptionRepository.GetByConsultationIdAsync(
                request.ConsultationId,
                cancellationToken
                );

            if (existingPrescription != null)
            {
                throw new BadRequestException(
                    "A prescription already exists for this consultation.");
            }

            var prescription = _mapper.Map<Prescription>(request);

            prescription.Id = Guid.NewGuid();
            prescription.ConsultationId = consultation.Id;
            prescription.PatientId = consultation.PatientId;
            prescription.DoctorId = consultation.DoctorId;
            prescription.CreatedAt = DateTimeOffset.UtcNow;

            foreach (var item in prescription.Items)
            {
                item.Id = Guid.NewGuid();
                item.PrescriptionId = prescription.Id;
                item.CreatedAt = DateTimeOffset.UtcNow;

                item.MedicineName = item.MedicineName.Trim();
                item.Dosage = item.Dosage.Trim();
                item.Frequency = item.Frequency.Trim();
                item.Duration = item.Duration.Trim();

                item.GenericName = item.GenericName?.Trim();
                item.DosageForm = item.DosageForm?.Trim();
                item.Instructions = item.Instructions?.Trim();
            }

            prescription.Notes = prescription.Notes?.Trim();
            prescription.Diagnosis = prescription.Diagnosis?.Trim();
            prescription.TreatmentPlan = string.IsNullOrWhiteSpace(prescription.TreatmentPlan)
                ? null
                : prescription.TreatmentPlan.Trim();
            prescription.ClinicalNotes = string.IsNullOrWhiteSpace(prescription.ClinicalNotes)
                ? null
                : prescription.ClinicalNotes.Trim();

            await _prescriptionRepository.AddAsync(
                prescription,
                cancellationToken);

            await _prescriptionRepository.SaveChangesAsync(cancellationToken);

            try
            {
                await _messagePublisher.PublishAsync(
                    new PrescriptionPdfRequested(prescription.Id),
                    cancellationToken);

                _logger.LogInformation(
                    "PDF generation requested for prescription {PrescriptionId}",
                    prescription.Id);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Prescription {PrescriptionId} saved, but RabbitMQ publishing failed.",
                    prescription.Id);
            }

            var created = await _prescriptionRepository.GetByIdAsync(
                prescription.Id,
                cancellationToken) ?? prescription;

            return await EnrichDoctorAsync(
                MapToDto(created, _mapper),
                cancellationToken);

        }

        public async Task<PrescriptionResponseDto> GetByConsultationIdAsync(Guid consultationId, 
            Guid userId, string role, CancellationToken cancellationToken)
        {
            var consultation = await _consultationRepository.GetByIdAsync(
            consultationId,
            cancellationToken);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            if (role == "Patient")
            {
                if (consultation.PatientId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this prescription.");
                }
            }
            else if (role == "Doctor")
            {
                if (consultation.DoctorId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this prescription.");
                }
            }
            else
            {
                throw new ForbiddenException(
                    "You are not authorized to access this prescription.");
            }

            var prescription =
                await _prescriptionRepository.GetByConsultationIdAsync(
                    consultationId,
                    cancellationToken);

            if (prescription == null)
            {
                throw new NotFoundException(
                    "Prescription not found for this consultation.");
            }

            return await EnrichDoctorAsync(
                MapToDto(prescription, _mapper),
                cancellationToken);

        }

        public async Task<PrescriptionResponseDto> GetByIdAsync(Guid prescriptionId, 
            Guid userId, string role, CancellationToken cancellationToken)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(
            prescriptionId,
            cancellationToken);

            if (prescription == null)
            {
                throw new NotFoundException("Prescription not found.");
            }

            if (role == "Patient")
            {
                if (prescription.PatientId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this prescription.");
                }
            }
            else if (role == "Doctor")
            {
                if (prescription.DoctorId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this prescription.");
                }
            }
            else
            {
                throw new ForbiddenException(
                    "You are not authorized to access this prescription.");
            }

            return await EnrichDoctorAsync(
                MapToDto(prescription, _mapper),
                cancellationToken);

        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetMyPrescriptionsAsync(Guid patientId, 
            CancellationToken cancellationToken)
        {
       

            var prescriptions =
                await _prescriptionRepository.GetByPatientIdAsync(
                    patientId,
                    cancellationToken);

            var dtos = prescriptions
                .Select(p => MapToDto(p, _mapper))
                .ToList();

            await EnrichDoctorAsync(dtos, cancellationToken);

            return dtos;

        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetDoctorPrescriptionsAsync(Guid doctorId,
            CancellationToken cancellationToken)
        {
            var prescriptions =
                await _prescriptionRepository.GetByDoctorIdAsync(
                    doctorId,
                    cancellationToken);

            var dtos = prescriptions
                .Select(p => MapToDto(p, _mapper))
                .ToList();

            await EnrichDoctorAsync(dtos, cancellationToken);

            return dtos;
        }

        private static PrescriptionResponseDto MapToDto(
            Prescription prescription,
            IMapper mapper)
        {
            var dto = mapper.Map<PrescriptionResponseDto>(prescription);

            var record = prescription.Consultation?.MedicalRecord;

            if (string.IsNullOrWhiteSpace(dto.Diagnosis))
            {
                dto.Diagnosis = record?.Diagnosis;
            }

            if (string.IsNullOrWhiteSpace(dto.TreatmentPlan))
            {
                dto.TreatmentPlan = record?.TreatmentPlan;
            }

            if (string.IsNullOrWhiteSpace(dto.ClinicalNotes))
            {
                dto.ClinicalNotes = record?.ClinicalNotes;
            }

            return dto;
        }

        private async Task<PrescriptionResponseDto> EnrichDoctorAsync(
            PrescriptionResponseDto dto,
            CancellationToken cancellationToken)
        {
            await EnrichDoctorAsync(
                new List<PrescriptionResponseDto> { dto },
                cancellationToken);

            return dto;
        }

        private async Task<List<PrescriptionResponseDto>> EnrichDoctorAsync(
            List<PrescriptionResponseDto> dtos,
            CancellationToken cancellationToken)
        {
            if (dtos.Count == 0)
                return dtos;

            var cache = new Dictionary<(Guid DoctorId, Guid PatientId), (string? DoctorName, string? Specialization, string? PatientName, string? DoctorProfileImageUrl)>();

            foreach (var dto in dtos)
            {
                var key = (dto.DoctorId, dto.PatientId);

                if (!cache.TryGetValue(key, out var info))
                {
                    info = await FetchDoctorInfoAsync(
                        dto.DoctorId,
                        dto.PatientId,
                        cancellationToken);

                    cache[key] = info;
                }

                dto.DoctorName = info.DoctorName;
                dto.DoctorSpecialization = info.Specialization;
                dto.PatientName = info.PatientName;
                dto.DoctorProfileImageUrl = info.DoctorProfileImageUrl;
            }

            return dtos;
        }

        private async Task<(string? DoctorName, string? Specialization, string? PatientName, string? DoctorProfileImageUrl)> FetchDoctorInfoAsync(
            Guid doctorId,
            Guid patientId,
            CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _userProfileClient.GetPrescriptionProfileAsync(
                    doctorId,
                    patientId,
                    cancellationToken);

                if (profile == null)
                    return (null, null, null, null);

                var doctorName =
                    $"{profile.Doctor?.FirstName} {profile.Doctor?.LastName}".Trim();

                var patientName =
                    $"{profile.Patient?.FirstName} {profile.Patient?.LastName}".Trim();

                return (
                    string.IsNullOrWhiteSpace(doctorName) ? null : doctorName,
                    profile.Doctor?.SpecializationName,
                    string.IsNullOrWhiteSpace(patientName) ? null : patientName,
                    profile.Doctor?.ProfileImageUrl);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // Doctor info is best-effort; never fail the prescription response.
                return (null, null, null, null);
            }
        }
    }
}
