using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface ILabResultPdfService
    {
        Task UploadAsync(
             Guid labOrderId,
            Guid labUserId,
            Stream fileStream,
            string fileName,
            CancellationToken cancellationToken);

        Task<(byte[] Content, string FileName)> DownloadAsync(
            Guid labResultId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);
    }
}
