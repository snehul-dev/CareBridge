using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Doctors;
using UserService.Application.Interfaces;
using CareBridge.Shared.Exceptions;
using AutoMapper;

namespace UserService.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper; 

        public DoctorService(IDoctorRepository doctorRepository , 
            IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            
        }
        public async Task<DoctorSearchResponse> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {

            var doctor = await _doctorRepository.GetVerifiedByIdAsync(doctorId, cancellationToken);
            if(doctor == null)
            {
                throw new NotFoundException("Verified doctor not found.");
            }
           return _mapper.Map<DoctorSearchResponse>(doctor);
            
         }

        public async Task<IReadOnlyList<DoctorSearchResponse>> SearchAsync(string? specialization, string? city, string? district, CancellationToken cancellationToken)
        {
            var doctors = await _doctorRepository.SearchAsync(specialization, city, district, cancellationToken);
            return _mapper.Map<List<DoctorSearchResponse>>(doctors);
        }
    }
}
