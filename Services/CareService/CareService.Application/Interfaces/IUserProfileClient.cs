using CareBridge.Contracts.Users;

namespace CareService.Application.Interfaces
{
    public interface IUserProfileClient
    {
        Task<UserPrescriptionProfileDto?> GetPrescriptionProfileAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }
}
