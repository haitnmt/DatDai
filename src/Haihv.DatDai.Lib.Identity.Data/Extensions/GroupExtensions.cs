using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Data.Extensions;

public static class GroupExtensions
{
    /// <summary>
    /// Chuyển đổi từ <see cref="GroupLdap"/> sang <see cref="Group"/>.
    /// </summary>
    /// <param name="groupLdap"></param>
    /// <returns>
    /// Trả về một đối tượng <see cref="Group"/> mới.
    /// </returns>
    public static Group ToGroup(this GroupLdap groupLdap)
    {
        return new Group
        {
            Id = groupLdap.Id,
            GroupName = groupLdap.Cn ?? string.Empty,
            DistinguishedName = groupLdap.DistinguishedName,
            GroupType = 1,
            CreatedAt = groupLdap.WhenCreated,
            UpdatedAt = groupLdap.WhenChanged ?? groupLdap.WhenCreated,
        };
    }

    /// <summary>
    /// Tạo chuỗi hash từ thông tin nhóm.
    /// </summary>
    /// <param name="group"><see cref="Group"/></param>
    /// <returns>
    /// Chuỗi hash của thông tin nhóm.
    /// </returns>
    public static string? HashGroup(this Group group)
    {
        var infoString = $"{group.GroupName}_{group.DistinguishedName}" +
                         $"_{string.Join("-", group.MemberOf.Select(x => x.ToString()))}" +
                         $"_{group.GhiChu}_{group.IsDeleted}_{group.DeletedAtUtc}";
        return infoString.ComputeHash();
    }
}