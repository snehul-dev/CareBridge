using CareBridge.Shared.Exceptions;
using CareBridge.Contracts.Users;
using CareService.Application.DTOs.Prescriptions;
using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CareService.Application.Services
{
    public class PrescriptionPdfService : IPrescriptionPdfService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IUserProfileClient _userProfileClient;
        private readonly IPrescriptionPdfGenerator _pdfGenerator;
        private readonly IFileStorageService _fileStorageService;

        public PrescriptionPdfService(
            IPrescriptionRepository prescriptionRepository,
            IUserProfileClient userProfileClient,
            IPrescriptionPdfGenerator pdfGenerator,
            IFileStorageService fileStorageService)
        {
            _prescriptionRepository = prescriptionRepository;
            _userProfileClient = userProfileClient;
            _pdfGenerator = pdfGenerator;
            _fileStorageService = fileStorageService;
        }

        public async Task GenerateAndStoreAsync(Guid prescriptionId,
            Guid userId, string role, CancellationToken cancellationToken)
        {
            var prescription = await GetAuthorizedPrescriptionAsync(
                prescriptionId, userId, role, cancellationToken);

            var profile = await _userProfileClient.GetPrescriptionProfileAsync(
                prescription.DoctorId,
                prescription.PatientId,
                cancellationToken);

            if (profile == null)
            {
                throw new NotFoundException(
                    "Doctor or patient profile not found for this prescription.");
            }

            var pdfDto = BuildPdfDto(prescription, profile);
            var pdfBytes = _pdfGenerator.Generate(pdfDto);

            var storageKey = await _fileStorageService.SaveAsync(
                pdfBytes,
                $"{prescription.Id}.pdf",
                "application/pdf",
                "prescriptions",
                cancellationToken);

            prescription.PdfStorageKey = storageKey;
            prescription.PdfGeneratedAt = DateTimeOffset.UtcNow;

            await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<byte[]> GetPdfAsync(Guid prescriptionId,
            Guid userId, string role, CancellationToken cancellationToken)
        {
            var prescription = await GetAuthorizedPrescriptionAsync(
                prescriptionId, userId, role, cancellationToken);

            if (string.IsNullOrWhiteSpace(prescription.PdfStorageKey))
            {
                throw new NotFoundException(
                    "PDF has not been generated for this prescription.");
            }

            return await _fileStorageService.GetAsync(
                prescription.PdfStorageKey,
                cancellationToken);
        }

        private async Task<Prescription> GetAuthorizedPrescriptionAsync(
            Guid prescriptionId,
            Guid userId,
            string role,
            CancellationToken cancellationToken)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(
                prescriptionId,
                cancellationToken);

            if (prescription == null)
            {
                throw new NotFoundException("Prescription not found.");
            }

            var isOwner = role switch
            {
                "Patient" => prescription.PatientId == userId,
                "Doctor" => prescription.DoctorId == userId,
                _ => false
            };

            if (!isOwner)
            {
                throw new ForbiddenException(
                    "You are not authorized to access this prescription.");
            }

            return prescription;
        }

        private static PrescriptionPdfDto BuildPdfDto(
            Prescription prescription,
            PrescriptionProfileResponseDto profile)
        {
            var doctor = profile.Doctor;
            var patient = profile.Patient;

            var doctorAddress = string.Join(", ", new[]
            {
                doctor.DoctorAddress,
                doctor.City
            }.Where(part => !string.IsNullOrWhiteSpace(part)));

            var record = prescription.Consultation?.MedicalRecord;

            return new PrescriptionPdfDto
            {
                PrescriptionId = prescription.Id,
                CreatedAt = prescription.CreatedAt,

                DoctorName =
                    $"{doctor.FirstName} {doctor.LastName}".Trim(),
                Qualification = doctor.Qualification ?? string.Empty,
                Specialization = doctor.SpecializationName ?? string.Empty,
                LicenseNumber = doctor.LicenseNumber ?? string.Empty,
                ClinicName = doctor.HospitalOrClinicName,
                DoctorAddress = doctorAddress,

                PatientName =
                    $"{patient.FirstName} {patient.LastName}".Trim(),
                PatientEmail = patient.Email ?? string.Empty,
                PatientPhone = patient.PhoneNumber ?? string.Empty,
                BloodGroup = patient.BloodGroup,

                Diagnosis = string.IsNullOrWhiteSpace(prescription.Diagnosis)
                    ? record?.Diagnosis
                    : prescription.Diagnosis,
                TreatmentPlan = string.IsNullOrWhiteSpace(prescription.TreatmentPlan)
                    ? record?.TreatmentPlan
                    : prescription.TreatmentPlan,
                ClinicalNotes = string.IsNullOrWhiteSpace(prescription.ClinicalNotes)
                    ? record?.ClinicalNotes
                    : prescription.ClinicalNotes,

                Notes = prescription.Notes,

                Items = prescription.Items
                    .Select(item => new PrescriptionPdfItemDto
                    {
                        MedicineName = item.MedicineName,
                        GenericName = item.GenericName,
                        DosageForm = item.DosageForm,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        Duration = item.Duration,
                        Instructions = item.Instructions,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };
        }

        public async Task GenerateInternalAsync(Guid prescriptionId, CancellationToken cancellationToken)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(
                    prescriptionId,
                    cancellationToken);

            if (prescription == null)
                throw new NotFoundException("Prescription not found.");

            // Already generated? Skip duplicate processing.
            if (!string.IsNullOrWhiteSpace(prescription.PdfStorageKey))
                return;

            var profile = await _userProfileClient.GetPrescriptionProfileAsync(
                prescription.DoctorId,
                prescription.PatientId,
                cancellationToken);

            if (profile == null)
                throw new NotFoundException(
                    "Doctor or patient profile not found.");

            var pdfDto = BuildPdfDto(prescription, profile);

            var pdfBytes = _pdfGenerator.Generate(pdfDto);

            var storageKey = await _fileStorageService.SaveAsync(
                pdfBytes,
                $"{prescription.Id}.pdf",
                "application/pdf",
                "prescriptions",
                cancellationToken);

            prescription.PdfStorageKey = storageKey;
            prescription.PdfGeneratedAt = DateTimeOffset.UtcNow;

            await _prescriptionRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
