using CareBridge.Contracts.Users;

namespace CareService.Application.Interfaces
{
    public interface IUserProfileClient
    {
        Task<PrescriptionProfileResponseDto?> GetPrescriptionProfileAsync(
            Guid doctorUserId,
            Guid patientUserId,
            CancellationToken cancellationToken);
    }
}
