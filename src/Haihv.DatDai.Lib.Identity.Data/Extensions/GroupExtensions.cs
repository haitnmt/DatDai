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
            GroupType = 1,
            WhenCreated = groupLdap.WhenCreated,
            WhenChanged = groupLdap.WhenChanged
        };
    }
public static string? HashGroup(this Group group)
{
    var infoString = $"{group.GroupName}_{string.Join("-", group.MemberOf.Select(x => x.ToString()))}";
    infoString += $"_{group.WhenChanged}_{group.GhiChu}_{group.DeletedAtUtc}";
    return infoString.ComputeHash();
}
}