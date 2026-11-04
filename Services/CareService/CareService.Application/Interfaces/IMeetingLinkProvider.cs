using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IMeetingLinkProvider
    {
        Task<string> GenerateMeetingLinkAsync(Guid consultationId,
            CancellationToken cancellationToken);
    }
}
