using System;

namespace CareService.Application.Interfaces.Services
{
    public interface IInternalLabClient
    {
        Task<string?> GetLabNameAsync(
            Guid labId,
            CancellationToken cancellationToken);
    }
}
