using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(
        byte[] fileContent,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken);

        Task<byte[]> GetAsync(
            string storageKey,
            CancellationToken cancellationToken);
    }
}
