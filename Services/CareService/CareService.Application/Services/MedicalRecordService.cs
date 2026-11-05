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
        private readonly IMapper _mapper;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository,
            IConsultationRepository consultationRepository,
            IMapper mapper
            )
        {
            _medicalRecordRepository = medicalRecordRepository;
            _consultationRepository = consultationRepository;
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

            return _mapper.Map<MedicalRecordResponseDto>(medicalRecord);




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

            return _mapper.Map<MedicalRecordResponseDto>(record);
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

            return _mapper.Map<IEnumerable< MedicalRecordResponseDto>>(records);

        }
    }
}
