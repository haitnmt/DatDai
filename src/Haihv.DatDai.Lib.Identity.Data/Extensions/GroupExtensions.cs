using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Data.Extensions;

public static class GroupExtensions
{
    public static Group ToGroup(this GroupLdap groupLdap)
    {
        return new Group
        {
            Id = groupLdap.Id,
            GroupName = groupLdap.Cn ?? string.Empty,
            WhenCreated = groupLdap.WhenCreated,
            WhenChanged = groupLdap.WhenChanged
        };
    }
    public static string? HashGroup(this Group group)
    {
        var infoString =
            $"{group.Id}_{group.GroupName}_{group.MemberOf.Select(x =>x.ToString())}_{group.WhenCreated}_{group.WhenChanged}_{group.GhiChu}_{group.DeletedAtUtc}";
        return infoString.ComputeHash();
    }
}