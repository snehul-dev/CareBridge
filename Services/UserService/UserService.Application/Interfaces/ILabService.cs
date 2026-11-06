using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Labs;

namespace UserService.Application.Interfaces
{
    public interface ILabService
    {
        Task<IEnumerable<LabResponseDto>> GetVerifiedLabsAsync(
            CancellationToken cancellationToken);

        Task<LabResponseDto> GetVerifiedLabByIdAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task<LabVerificationResponseDto> CheckLabVerificationAsync(
            Guid labId,
            CancellationToken cancellationToken);
    }
}
 