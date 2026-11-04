using AutoMapper;
using CareBridge.Contracts.Appointments;
using CareBridge.Shared;
using CareBridge.Shared.Exceptions;
using CareService.Application.DTOs.Consultations;
using CareService.Application.Interfaces;
using CareService.Domain.Entities;
using CareService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Services
{
    public class ConsultationService : IConsultationService
    {

        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;
        private readonly IAppointmentServiceClient _appointmentServiceClient;
        private readonly IMeetingLinkProvider _meetingLinkProvider;

        public ConsultationService(IConsultationRepository consultationRepository,
            IMapper mapper,
            IAppointmentServiceClient appointmentServiceClient,
            IMeetingLinkProvider meetingLinkProvider
            )
        {
            _consultationRepository = consultationRepository;
            _mapper = mapper;
            _appointmentServiceClient = appointmentServiceClient;
            _meetingLinkProvider = meetingLinkProvider;

        }
        public async Task<ConsultationDetailsResponseDto> CompleteAsync(Guid consultationId, Guid doctorId ,
            CancellationToken cancellationToken)
        {

            var consultation = await _consultationRepository.GetByIdAsync(consultationId, cancellationToken);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            //verify doctor

            if (consultation.DoctorId != doctorId)
            {
                throw new ForbiddenException("You are not authorized to Complete this consultation.");
            }

            //validate consultation status

            if (consultation.Status != ConsultationStatus.InProgress)
            {
                throw new BadRequestException("Only ongoing consultations can be completed.");
            }

            consultation.Status = ConsultationStatus.Completed;
            consultation.EndedAt = DateTimeOffset.UtcNow;
            consultation.UpdatedAt = DateTimeOffset.UtcNow;

            _consultationRepository.Update(consultation);
            await _consultationRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ConsultationDetailsResponseDto>(consultation);
        }

        public async Task<ConsultationDetailsResponseDto> CreateAsync(CreateConsultationRequestDto request, Guid doctorId,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(typeof(ConsultationType), request.ConsultationType))
            {
                throw new BadRequestException("Invalid consultation type.");
            }

            var appointment = await _appointmentServiceClient.GetAppointmentByIdAsync(request.AppointmentId,
                cancellationToken);

            if(appointment == null)
            {
                throw new NotFoundException("Appointment not found");
            }
            if(appointment.DoctorId != doctorId)
            {
                throw new ForbiddenException("You are not assigned to this appointment.");
            }
            if (appointment.Status != AppointmentStatusNames.Confirmed )
            {
                throw new BadRequestException(
                    "Consultation can only be created for confirmed appointments.");
            }

            var existingConsultation = await _consultationRepository.GetByAppointmentIdAsync(request.AppointmentId,
                cancellationToken);

            if(existingConsultation != null)
            {
                throw new BadRequestException("A consultation already exists for this appointment.");
            }

            var consultationId = Guid.NewGuid();
            string? meetingLink = null;

            if(request.ConsultationType == ConsultationType.Online)
            {
                meetingLink = await _meetingLinkProvider.GenerateMeetingLinkAsync(consultationId,
                    cancellationToken);


                if (string.IsNullOrWhiteSpace(meetingLink))
                {
                    throw new BadRequestException(
                        "Unable to generate meeting link.");
                }
            }

            var consultation = new Consultation
            {
                Id = consultationId,
                AppointmentId = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                ConsultationType = request.ConsultationType,
                Status = ConsultationStatus.Scheduled,
                MeetingLink = meetingLink

            };

            await _consultationRepository.AddAsync(consultation, cancellationToken);
            await _consultationRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ConsultationDetailsResponseDto>(consultation);


        }

        public async Task<IEnumerable<ConsultationDetailsResponseDto>> GetDoctorConsultationsAsync(Guid doctorId,
            CancellationToken cancellationToken)
        {
            var consultations = await _consultationRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
      

            return  _mapper.Map<IEnumerable <ConsultationDetailsResponseDto>>(consultations);
        }

        public async Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultaionAsync(Guid patientId,
            CancellationToken cancellationToken)
        {
            var consultations = await _consultationRepository.GetByPatientIdAsync(patientId, cancellationToken);
       

            return _mapper.Map<IEnumerable<ConsultationDetailsResponseDto>>(consultations);

        }

        public async Task<IEnumerable<ConsultationDetailsResponseDto>> GetPatientConsultationsForDoctorAsync(Guid patientId, Guid doctorId, CancellationToken cancellationToken)
        {
            var consultation = await _consultationRepository.GetByPatientAndDoctorAsync(patientId,
                doctorId, cancellationToken);
            return  _mapper.Map<IEnumerable<ConsultationDetailsResponseDto>>(consultation);
        }

        public async Task<ConsultationDetailsResponseDto> GetByIdAsync(Guid consultationId, Guid userId, string role,
            CancellationToken cancellationToken)
        {
            var consultation = await _consultationRepository.GetByIdAsync(
                        consultationId,
                        cancellationToken);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            // Verify ownership based on role
            if (role == "Doctor")
            {
                if (consultation.DoctorId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this consultation.");
                }
            }
            else if (role == "Patient")
            {
                if (consultation.PatientId != userId)
                {
                    throw new ForbiddenException(
                        "You are not authorized to access this consultation.");
                }
            }
            else
            {
                throw new ForbiddenException(
                    "You are not authorized to access this consultation.");
            }

            return _mapper.Map<ConsultationDetailsResponseDto>(consultation);

        }

        public async Task<ConsultationDetailsResponseDto> StartAsync(Guid consultationId, Guid doctorId,
            CancellationToken cancellationToken)
        {
            var consultation = await _consultationRepository.GetByIdAsync(consultationId, cancellationToken);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation not found.");
            }

            //verify doctor

            if (consultation.DoctorId != doctorId)
            {
                throw new ForbiddenException("You are not authorized to start this consultation.");
            }

            //validate consultation status

            if (consultation.Status != ConsultationStatus.Scheduled)
            {
                throw new BadRequestException("Only scheduled consultations can be started.");
            }

            if (consultation.ConsultationType == ConsultationType.Online &&
                    string.IsNullOrWhiteSpace(consultation.MeetingLink))
            {
                throw new BadRequestException(
                    "Meeting link is not available.");
            }

            consultation.Status = ConsultationStatus.InProgress;
            consultation.StartedAt = DateTimeOffset.UtcNow;
            consultation.UpdatedAt = DateTimeOffset.UtcNow;

            _consultationRepository.Update(consultation);
            await _consultationRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ConsultationDetailsResponseDto>(consultation);

        }
    }
}
