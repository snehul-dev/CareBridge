namespace CareService.Application.Interfaces
{
    public interface IPrescriptionPdfService
    {
        Task GenerateAndStoreAsync(
            Guid prescriptionId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);

        Task GenerateInternalAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken);

        Task<byte[]> GetPdfAsync(
            Guid prescriptionId,
            Guid userId,
            string role,
            CancellationToken cancellationToken);
    }
}
