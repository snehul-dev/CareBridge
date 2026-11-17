using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces
{
    public interface IProfilePictureService
    {
        Task<string> UploadProfilePictureAsync(
            Guid userId,
            byte[] content,
            CancellationToken cancellationToken);

        Task<ProfilePictureContent> GetProfilePictureAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }

    public sealed record ProfilePictureContent(
        byte[] Content,
        string ContentType);
}
