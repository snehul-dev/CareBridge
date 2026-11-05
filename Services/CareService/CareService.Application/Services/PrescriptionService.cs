using AutoMapper;
using CareBridge.Shared.Exceptions;
using CareService.Application.DTOs.Prescriptions;
using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;
        public PrescriptionService(IPrescriptionRepository prescriptionRepository , 
            IConsultationRepository consultationRepository,
            IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _consultationRepository = consultationRepository;
            _mapper = mapper;
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

            await _prescriptionRepository.AddAsync(
                prescription,
                cancellationToken);

            await _prescriptionRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PrescriptionResponseDto>(prescription);

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

            return _mapper.Map<PrescriptionResponseDto>(prescription);

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

            return _mapper.Map<PrescriptionResponseDto>(prescription);

        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetMyPrescriptionsAsync(Guid patientId, 
            CancellationToken cancellationToken)
        {
       

            var prescriptions =
                await _prescriptionRepository.GetByPatientIdAsync(
                    patientId,
                    cancellationToken);

            return _mapper.Map<IEnumerable<PrescriptionResponseDto>>(
                prescriptions);

        }
    }
}
