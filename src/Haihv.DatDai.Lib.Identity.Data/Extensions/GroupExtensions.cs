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
}