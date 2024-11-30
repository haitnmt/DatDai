using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Data.Extensions;

public static class UserExtensions
{
    /// <summary>
    /// Kiểm tra xem người dùng có phải là người dùng hệ thống không.
    /// </summary>
    /// <param name="user">Người dùng cần kiểm tra.</param>
    /// <returns>Trả về <c>true</c> nếu người dùng là người dùng hệ thống, ngược lại trả về <c>false</c>.</returns>
    public static bool IsSystemUser(this User user)
    {
        return user.AuthenticationType == 0;
    }

    /// <summary>
    /// Chuyển đổi từ <see cref="UserLdap"/> sang <see cref="User"/>.
    /// </summary>
    /// <param name="userLdap"></param>
    /// <returns>
    /// Trả về một đối tượng <see cref="User"/> mới.
    /// </returns>
    public static User ToUser(this UserLdap userLdap)
    {
        return new User
        {
            Id = userLdap.Id,
            UserName = userLdap.UserPrincipalName ?? string.Empty,
            Email = userLdap.Email ?? string.Empty,
            DisplayName = userLdap.DisplayName ?? string.Empty,
            JobTitle = userLdap.JobTitle,
            Description = userLdap.Description,
            AuthenticationType = 1,
            IsLocked = userLdap.IsLocked,
            IsPwdMustChange = userLdap.IsPwdMustChange,
            PwdLastSet = userLdap.PwdLastSet,
            WhenCreated = userLdap.WhenCreated,
            WhenChanged = userLdap.WhenChanged
        };
    }
}
