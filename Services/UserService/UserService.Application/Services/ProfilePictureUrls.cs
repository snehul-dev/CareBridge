using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Services
{
    public static class ProfilePictureUrls
    {
        public static string Build(Guid userId, DateTime? version)
        {
            var ticks = version?.Ticks ?? 0;
            return $"/api/users/Users/{userId}/profile-picture?v={ticks}";
        }
    }
}
