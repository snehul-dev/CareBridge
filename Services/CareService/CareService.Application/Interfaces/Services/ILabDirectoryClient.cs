using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces.Services
{
    public interface ILabDirectoryClient
    {
        Task<bool> IsVerifiedLabAsync(
            Guid labId,
            CancellationToken cancellationToken);
    }
}
