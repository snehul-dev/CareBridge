using CareService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.ExternalServices
{
    public class JitsiMeetingLinkProvider : IMeetingLinkProvider
    {
        public Task<string> GenerateMeetingLinkAsync(Guid consultationId,
            CancellationToken cancellationToken)
        {
            var meetingLink = $"https://meet.jit.si/CareBridge-{consultationId:N}";

            return Task.FromResult(meetingLink);
        }
    }
}
