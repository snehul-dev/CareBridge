using AutoMapper;
using CareBridge.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Labs;
using UserService.Application.Interfaces;
using CareBridge.Contracts.Labs;

namespace UserService.Application.Services
{
    public class LabService : ILabService
    {
        private readonly ILabRepository _labRepository;
        private readonly IMapper _mapper;
        public LabService(ILabRepository labRepository , 
            IMapper mapper)
        {
            _labRepository = labRepository;
            _mapper = mapper;
        }

        public async Task<LabVerificationResponseDto> CheckLabVerificationAsync(Guid labId,
            CancellationToken cancellationToken)
        {
            var isVerified = await _labRepository.IsActiveAndVerifiedAsync(labId, cancellationToken);
            return new LabVerificationResponseDto
            {
                IsVerified = isVerified
            };
        }

        public async Task<LabResponseDto> GetVerifiedLabByIdAsync(Guid labId, CancellationToken cancellationToken)
        {
            var lab = await _labRepository.GetVerifiedLabByIdAsync(
                    labId,
                    cancellationToken);

            if (lab == null || !lab.IsVerified)
                throw new NotFoundException(
                    "Verified lab not found.");

            return _mapper.Map<LabResponseDto>(lab);
        }

        public async Task<IEnumerable<LabResponseDto>> GetVerifiedLabsAsync(CancellationToken cancellationToken)
        {
            var labs = await _labRepository.GetVerifiedLabsAsync(cancellationToken);
            

            return _mapper.Map<IEnumerable<LabResponseDto>>(labs);
        }
    }
}
