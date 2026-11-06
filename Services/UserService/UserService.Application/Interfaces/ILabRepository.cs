using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface ILabRepository
    {
        Task<List<LabProfile>> GetVerifiedLabsAsync(
             CancellationToken cancellationToken);

        Task<LabProfile?> GetVerifiedLabByIdAsync(
            Guid labId,
            CancellationToken cancellationToken);

        Task<bool> IsActiveAndVerifiedAsync(
            Guid labId,
            CancellationToken cancellationToken);
    }
}
