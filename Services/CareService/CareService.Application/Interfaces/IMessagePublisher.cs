using CareBridge.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(PrescriptionPdfRequested message,
            CancellationToken cancellationToken);
    }
}
