using CareBridge.Shared.Exceptions;
using CareService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Application.Services
{
    public class LabResultPdfService:ILabResultPdfService
    {
        private readonly ILabResultRepository _labResultRepository;
        private readonly IFileStorageService _fileStorageService;

        private const long MaxFileSize = 10 * 1024 * 1024;

        public LabResultPdfService(ILabResultRepository labResultRepository, 
             IFileStorageService fileStorageService)
        {
            _labResultRepository = labResultRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task UploadAsync(Guid labResultId, Guid labUserId, Stream fileStream, 
            string fileName, CancellationToken cancellationToken)
        {
            var labResult = await _labResultRepository.GetByIdWithLabOrderAsync(labResultId, cancellationToken);

            if (labResult == null)
                throw new NotFoundException("Lab result not found.");

            var labOrder = labResult.LabOrderItem.LabOrder;

            if (labOrder.AssignedLabId != labUserId)
                throw new ForbiddenException(
                    "You are not assigned to this lab order.");

            // 3. Validate file
            if (fileStream == null || !fileStream.CanRead)
                throw new ArgumentException("Invalid file.");

            if (fileStream.CanSeek && fileStream.Length == 0)
                throw new ArgumentException("File is empty.");

            if (fileStream.CanSeek && fileStream.Length > MaxFileSize)
                throw new ArgumentException("Maximum file size is 10 MB.");

            if (string.IsNullOrWhiteSpace(fileName) ||
                !string.Equals(
                    Path.GetExtension(fileName),
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only PDF files are allowed.");
            }

            // 4. Read stream
            using var memoryStream = new MemoryStream();

            await fileStream.CopyToAsync(
                memoryStream,
                cancellationToken);

            var fileContent = memoryStream.ToArray();

            if (fileContent.Length == 0)
                throw new ArgumentException("File is empty.");

            if (fileContent.Length > MaxFileSize)
                throw new ArgumentException("Maximum file size is 10 MB.");

            // 5. Validate actual PDF signature
            byte[] pdfSignature = { 0x25, 0x50, 0x44, 0x46, 0x2D };

            if (fileContent.Length < pdfSignature.Length ||
                !fileContent.Take(pdfSignature.Length)
                    .SequenceEqual(pdfSignature))
            {
                throw new ArgumentException("Invalid PDF file.");
            }

            // 6. Upload to AWS S3
            var safeFileName = Path.GetFileName(fileName);

            var storageKey = await _fileStorageService.SaveAsync(
                fileContent,
                $"{labResultId}.pdf",
                "application/pdf",
                "lab-reports",
                cancellationToken);

            // 7. Save PDF metadata
            labResult.PdfStorageKey = storageKey;
            labResult.PdfFileName = safeFileName;
            labResult.ContentType = "application/pdf";
            labResult.FileSize = fileContent.Length;
            labResult.UploadedBy = labUserId;
            labResult.UploadedAt = DateTimeOffset.UtcNow;

            await _labResultRepository.UpdateAsync(
                labResult,
                cancellationToken);

            await _labResultRepository.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<(byte[] Content, string FileName)> DownloadAsync(Guid labResultId, Guid userId, string role, CancellationToken cancellationToken)
        {
            var labResult = await _labResultRepository
                .GetByIdWithLabOrderAsync(
                    labResultId,
                    cancellationToken);

            if (labResult == null)
                throw new NotFoundException("Lab result not found.");

            // 2. Verify user authorization
            var labOrder = labResult.LabOrderItem.LabOrder;

            var normalizedRole = role?.Trim().ToLowerInvariant();

            bool isPatient =
                normalizedRole == "patient" &&
                labOrder.PatientId == userId;

            bool isDoctor =
                normalizedRole == "doctor" &&
                labOrder.DoctorId == userId;

            if (!isPatient && !isDoctor)
                throw new ForbiddenException(
                    "You are not authorized to download this report.");

            // 3. Check PDF availability
            if (string.IsNullOrWhiteSpace(labResult.PdfStorageKey))
                throw new NotFoundException(
                    "Lab report PDF has not been uploaded yet.");

            // 4. Download from S3
            var content = await _fileStorageService.GetAsync(
                labResult.PdfStorageKey,
                cancellationToken);

            var fileName = string.IsNullOrWhiteSpace(labResult.PdfFileName)
                ? "LabReport.pdf"
                : labResult.PdfFileName;

            return (content, fileName);
        }
    }
}
