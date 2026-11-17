using Amazon.S3;
using Amazon.S3.Model;
using CareService.Application.Interfaces;
using CareService.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure.Storage
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _settings;

        public S3FileStorageService(IAmazonS3 s3Client ,
             IOptions<AwsSettings> options)
        {
            _s3Client = s3Client;
            _settings = options.Value;
        }
        public async Task<byte[]> GetAsync(string storageKey, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey
            };

            using var response = await _s3Client.GetObjectAsync(
                request,
                cancellationToken);

            using var memoryStream = new MemoryStream();

            await response.ResponseStream.CopyToAsync(
                memoryStream,
                cancellationToken);

            return memoryStream.ToArray();
        }

        public async Task<string> SaveAsync(byte[] fileContent, 
            string fileName, string contentType, string folder,
            CancellationToken cancellationToken)
        {
            var safeFileName = Path.GetFileName(fileName);

            var storageKey =
                $"{folder.TrimEnd('/')}/{safeFileName}";

            using var stream = new MemoryStream(fileContent);

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey,
                InputStream = stream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(
                request,
                cancellationToken);

            return storageKey;
        }
    }
}
