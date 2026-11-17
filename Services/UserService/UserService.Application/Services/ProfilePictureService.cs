using CareBridge.Shared.Exceptions;
using UserService.Application.Interfaces;

namespace UserService.Application.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        private const int MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorage;

        public ProfilePictureService(
            IUserRepository userRepository,
            IFileStorageService fileStorage)
        {
            _userRepository = userRepository;
            _fileStorage = fileStorage;
        }

        public async Task<string> UploadProfilePictureAsync(
            Guid userId,
            byte[] content,
            CancellationToken cancellationToken)
        {
            if (content.Length == 0)
                throw new BadRequestException(
                    "The selected file is empty.");

            if (content.Length > MaxFileSizeBytes)
                throw new BadRequestException(
                    "Profile picture must be 5 MB or smaller.");

            var extension = ResolveImageExtension(content);

            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            var newKey = $"profile-images/{userId}.{extension}";
            var oldKey = user.ProfileImageUrl;

            if (!string.IsNullOrWhiteSpace(oldKey) &&
                !string.Equals(oldKey, newKey, StringComparison.Ordinal))
            {
                try
                {
                    await _fileStorage.DeleteAsync(oldKey, cancellationToken);
                }
                catch
                {
                    // Best effort: an orphaned old image must not block the upload.
                }
            }

            await _fileStorage.SaveAsync(
                content,
                $"{userId}.{extension}",
                ExtensionToContentType(extension),
                "profile-images",
                cancellationToken);

            user.ProfileImageUrl = newKey;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(cancellationToken);

            return ProfilePictureUrls.Build(user.Id, user.UpdatedAt);
        }

        public async Task<ProfilePictureContent> GetProfilePictureAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                throw new NotFoundException("Profile picture not found.");

            var content = await _fileStorage.GetAsync(
                user.ProfileImageUrl,
                cancellationToken);

            return new ProfilePictureContent(
                content,
                ContentTypeFromKey(user.ProfileImageUrl));
        }

        private static string ResolveImageExtension(byte[] content)
        {
            if (IsJpeg(content))
                return "jpg";

            if (IsPng(content))
                return "png";

            if (IsWebp(content))
                return "webp";

            throw new BadRequestException(
                "Only JPEG, PNG and WEBP images are allowed.");
        }

        private static bool IsJpeg(byte[] content)
        {
            return content.Length >= 3 &&
                content[0] == 0xFF &&
                content[1] == 0xD8 &&
                content[2] == 0xFF;
        }

        private static bool IsPng(byte[] content)
        {
            return content.Length >= 8 &&
                content[0] == 0x89 &&
                content[1] == 0x50 &&
                content[2] == 0x4E &&
                content[3] == 0x47 &&
                content[4] == 0x0D &&
                content[5] == 0x0A &&
                content[6] == 0x1A &&
                content[7] == 0x0A;
        }

        private static bool IsWebp(byte[] content)
        {
            return content.Length >= 12 &&
                content[0] == (byte)'R' &&
                content[1] == (byte)'I' &&
                content[2] == (byte)'F' &&
                content[3] == (byte)'F' &&
                content[8] == (byte)'W' &&
                content[9] == (byte)'E' &&
                content[10] == (byte)'B' &&
                content[11] == (byte)'P';
        }

        private static string ExtensionToContentType(string extension)
        {
            return extension switch
            {
                "png" => "image/png",
                "webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        private static string ContentTypeFromKey(string storageKey)
        {
            var extension = Path.GetExtension(storageKey)
                .TrimStart('.')
                .ToLowerInvariant();

            return ExtensionToContentType(extension);
        }
    }
}
