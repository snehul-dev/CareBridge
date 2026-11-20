using AutoMapper;
using CareBridge.Shared.Exceptions;
using CareService.Application.DTOs.MedicalRecords;
using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IConsultationRepository _consultationRepository;
        private readonly IUserProfileClient _userProfileClient;
        private readonly IMapper _mapper;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository,
            IConsultationRepository consultationRepository,
            IUserProfileClient userProfileClient,
            IMapper mapper
            )
        {
            _medicalRecordRepository = medicalRecordRepository;
            _consultationRepository = consultationRepository;
            _userProfileClient = userProfileClient;
            _mapper = mapper;
        }
        public async Task<MedicalRecordResponseDto> CreateAsync(CreateMedicalRecordRequestDto request,
            Guid doctorId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Diagnosis))
            {
                throw new BadRequestException("Diagnosis is required.");
            }

            var consultation = await _consultationRepository.GetByIdAsync(request.ConsultationId,
                cancellationToken
                );

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            if (consultation.DoctorId != doctorId)
            {
                throw new ForbiddenException(
                    "You are not authorized to document this consultation.");
            }

            if (consultation.Status != ConsultationStatus.InProgress &&
            consultation.Status != ConsultationStatus.Completed)
            {
                throw new BadRequestException(
                    "Medical records can only be created for ongoing or completed consultations.");
            }

            var existingRecord = await _medicalRecordRepository.GetByConsultationIdAsync(request.ConsultationId, 
                cancellationToken);

            if(existingRecord != null)
            {
                throw new BadRequestException(
                "A medical record already exists for this consultation.");
            }

           var medicalRecord =  _mapper.Map<MedicalRecord>(request);

            medicalRecord.Id = Guid.NewGuid();
            medicalRecord.ConsultationId = consultation.Id;
            medicalRecord.PatientId = consultation.PatientId;
            medicalRecord.DoctorId = consultation.DoctorId;
            medicalRecord.CreatedAt = DateTimeOffset.UtcNow;

            await _medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);
            await _medicalRecordRepository.SaveChangesAsync(cancellationToken);

            return await EnrichDoctorAsync(
                _mapper.Map<MedicalRecordResponseDto>(medicalRecord),
                cancellationToken);




        }

        public async Task<MedicalRecordResponseDto> GetByIdAsync(Guid recordId, Guid userId, 
            string role, CancellationToken cancellationToken)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId, cancellationToken);

            if(record == null)
            {
                throw new NotFoundException("Medical record not found.");
            }

            if(role == "Patient")
            {
                if(record.PatientId != userId)
                {
                    throw new ForbiddenException(
                    "You are not authorized to access this medical record.");
                }
            }else if(role == "Doctor")
            {
                var consultations  = await _consultationRepository.GetByPatientAndDoctorAsync(
                    record.PatientId,
                    userId,
                    cancellationToken
                    );
                if (!consultations.Any())
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this medical record.");
                }

            }
            else
            {
                throw new ForbiddenException("You are not authorized to access this medical record.");
            }

            return await EnrichDoctorAsync(
                _mapper.Map<MedicalRecordResponseDto>(record),
                cancellationToken);
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetPatientHistoryAsync(Guid patientId,
            Guid userId, string role, CancellationToken cancellationToken)
        {
            if (role == "Patient")
            {
                if (patientId != userId)
                {
                    throw new ForbiddenException(
                        "You can only access your own medical history.");
                }
            }
            else if (role == "Doctor")
            {
                var consultations =
                    await _consultationRepository.GetByPatientAndDoctorAsync(
                        patientId,
                        userId,
                        cancellationToken);

                if (!consultations.Any())
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this patient's history.");
                }
            }
            else
            {
                throw new ForbiddenException(
                    "You are not authorized to access medical history.");
            }

            var records = await _medicalRecordRepository.GetByPatientIdAsync(patientId,
                cancellationToken
                );

            var dtos = _mapper.Map<List<MedicalRecordResponseDto>>(records);

            await EnrichDoctorAsync(dtos, cancellationToken);

            return dtos;

        }

        private async Task<MedicalRecordResponseDto> EnrichDoctorAsync(
            MedicalRecordResponseDto dto,
            CancellationToken cancellationToken)
        {
            await EnrichDoctorAsync(
                new List<MedicalRecordResponseDto> { dto },
                cancellationToken);

            return dto;
        }

        private async Task<List<MedicalRecordResponseDto>> EnrichDoctorAsync(
            List<MedicalRecordResponseDto> dtos,
            CancellationToken cancellationToken)
        {
            if (dtos.Count == 0)
                return dtos;

            var cache = new Dictionary<(Guid DoctorId, Guid PatientId), (string? Name, string? Specialization)>();

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

                dto.DoctorName = info.Name;
                dto.DoctorSpecialization = info.Specialization;
            }

            return dtos;
        }

        private async Task<(string? Name, string? Specialization)> FetchDoctorInfoAsync(
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

                if (profile?.Doctor == null)
                    return (null, null);

                var name =
                    $"{profile.Doctor.FirstName} {profile.Doctor.LastName}".Trim();

                return (
                    string.IsNullOrWhiteSpace(name) ? null : name,
                    profile.Doctor.SpecializationName);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // Doctor info is best-effort; never fail the medical record response.
                return (null, null);
            }
        }
    }
}
