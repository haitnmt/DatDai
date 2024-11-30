using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Enum;
using Haihv.DatDai.Lib.Identity.Ldap.Extension;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public class GroupLdapService(ILdapContext ldapContext)
{
    private readonly AttributeTypeLdap[] _attributesToReturns =
    [
        AttributeTypeLdap.ObjectGuid,
        AttributeTypeLdap.DistinguishedName,
        AttributeTypeLdap.SamAccountName,
        AttributeTypeLdap.Cn,
        AttributeTypeLdap.MemberOf,
        AttributeTypeLdap.Member
    ];
    
    private async Task<bool> ExistsAsync(string distinguishedName)
    {
        try
        {
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.DistinguishedName, [distinguishedName]);
            ResultEntryCollectionLdap resultEntryCollection = new(ldapContext);
            var entryCollection = await resultEntryCollection.GetAsync(attributeWithValueCollection, [AttributeTypeLdap.ObjectGuid]);
            return entryCollection is not null && entryCollection.Count != 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> ExistsAsync(Guid objectGuid)
    {
        try
        {
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.ObjectGuid, [objectGuid]);
            ResultEntryCollectionLdap resultEntryCollectionLdap = new(ldapContext);
            var entryCollection = await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, [AttributeTypeLdap.ObjectGuid]);
            return entryCollection is not null && entryCollection.Count != 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private async Task<List<GroupLdap>> GroupsLdapsFromSearchResultEntryCollection(SearchResultEntryCollection? resultEntrys)
    {
        List<GroupLdap> result = [];
        if (resultEntrys is null || resultEntrys.Count <= 0) return result;
        foreach (SearchResultEntry entry in resultEntrys)
        {
            GroupLdap groupsLdap = new()
            {
                Id = new Guid((byte[])entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.ObjectGuid)][0]),
                DistinguishedName = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.DistinguishedName)][0].ToString() ?? string.Empty,
                SamAccountName = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.SamAccountName)][0].ToString() ?? string.Empty,
                Cn = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Cn)][0].ToString() ?? string.Empty,
                MemberOf = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.MemberOf)]?.GetValues(typeof(string)).Cast<string>().ToHashSet() ?? []
            };
            HashSet<string> groupsMember = [];
            foreach (var distinguishedName in entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Member)]?.GetValues(typeof(string)).Cast<string>().ToArray() ?? [])
            {
                if (await ExistsAsync(distinguishedName))
                {
                    groupsMember.Add(distinguishedName);
                }
            }
            groupsLdap.GroupMembers = [.. groupsMember];
            result.Add(groupsLdap);
        }
        return result;
    }
    
    public async Task<GroupLdap?> GetDetailGroupsLdapModelByGroupDnAsync(string distinguishedName, DateTime whenChanged = default)
    {
        try
        {
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.DistinguishedName, [distinguishedName]);
            if (whenChanged != default)
                attributeWithValueCollection.Add(AttributeTypeLdap.WhenChanged, [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThan);
            ResultEntryCollectionLdap resultEntryCollectionLdap = new(ldapContext);
            var entryCollection = await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, _attributesToReturns);
            if (entryCollection is null || entryCollection.Count == 0) return null;
            return (await GroupsLdapsFromSearchResultEntryCollection(entryCollection))[0];
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<GroupLdap?> GetRootGroupAsync()
    {
        return await GetDetailGroupsLdapModelByGroupDnAsync(ldapContext.LdapConnectionInfo.RootGroupDn);
    }
}