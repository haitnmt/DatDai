using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Enum;
using Haihv.DatDai.Lib.Identity.Ldap.Extension;
using Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public class GroupLdapService(ILdapContext ldapContext) : IGroupLdapService
{
    private readonly LdapConnectionInfo _ldapConnectionInfo = ldapContext.LdapConnectionInfo;

    private readonly AttributeTypeLdap[] _attributesToReturns =
    [
        AttributeTypeLdap.ObjectGuid,
        AttributeTypeLdap.DistinguishedName,
        AttributeTypeLdap.SamAccountName,
        AttributeTypeLdap.Cn,
        AttributeTypeLdap.MemberOf,
        AttributeTypeLdap.Member,
        AttributeTypeLdap.WhenCreated,
        AttributeTypeLdap.WhenChanged
    ];

    private async Task<bool> ExistsAsync(string distinguishedName)
    {
        try
        {
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.DistinguishedName, [distinguishedName]);
            ResultEntryCollectionLdap resultEntryCollection = new(ldapContext);
            var entryCollection =
                await resultEntryCollection.GetAsync(attributeWithValueCollection, [AttributeTypeLdap.ObjectGuid]);
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
            var entryCollection =
                await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, [AttributeTypeLdap.ObjectGuid]);
            return entryCollection is not null && entryCollection.Count != 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<List<GroupLdap>> GroupLdapFromSearchResultEntryCollection(
        SearchResultEntryCollection? resultEntries)
    {
        List<GroupLdap> result = [];
        if (resultEntries is null || resultEntries.Count <= 0) return result;
        foreach (SearchResultEntry entry in resultEntries)
        {
            var whenCreatedString = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.WhenCreated)]?[0]
                .ToString();
            if (!DateTimeOffset.TryParseExact(whenCreatedString, "yyyyMMddHHmmss.0Z", null,
                    System.Globalization.DateTimeStyles.AssumeUniversal, out var whenCreated))
            {
                whenCreated = DateTimeOffset.MinValue;
            }

            var whenChangedString = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.WhenChanged)]?[0]
                .ToString();
            if (!DateTimeOffset.TryParseExact(whenChangedString, "yyyyMMddHHmmss.0Z", null,
                    System.Globalization.DateTimeStyles.AssumeUniversal, out var whenChanged))
            {
                whenChanged = DateTimeOffset.MinValue;
            }

            GroupLdap groupsLdap = new()
            {
                Id = new Guid((byte[])entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.ObjectGuid)][0]),
                DistinguishedName =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.DistinguishedName)][0].ToString() ??
                    string.Empty,
                SamAccountName =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.SamAccountName)][0].ToString() ??
                    string.Empty,
                Cn = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Cn)][0].ToString() ?? string.Empty,
                MemberOf = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.MemberOf)]
                    ?.GetValues(typeof(string)).Cast<string>().ToHashSet() ?? [],
                WhenCreated = whenCreated,
                WhenChanged = whenChanged
            };
            HashSet<string> groupsMember = [];
            foreach (var distinguishedName in entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Member)]
                         ?.GetValues(typeof(string)).Cast<string>().ToArray() ?? [])
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

    public async Task<GroupLdap?> GetDetailGroupsLdapModelByGroupDnAsync(string distinguishedName,
        DateTime whenChanged = default)
    {
        try
        {
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.DistinguishedName, [distinguishedName]);
            if (whenChanged != default)
                attributeWithValueCollection.Add(AttributeTypeLdap.WhenChanged,
                    [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThanOrEqual);
            ResultEntryCollectionLdap resultEntryCollectionLdap = new(ldapContext);
            var entryCollection =
                await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, _attributesToReturns);
            if (entryCollection is null || entryCollection.Count == 0) return null;
            return (await GroupLdapFromSearchResultEntryCollection(entryCollection))[0];
        }
        catch (Exception)
        {
            return null;
        }
    }


    public async Task<GroupLdap?> GetRootGroupAsync()
    {
        return await GetDetailGroupsLdapModelByGroupDnAsync(_ldapConnectionInfo.RootGroupDn);
    }

    /// <summary>
    /// Lấy tất cả các nhóm LDAP đã thay đổi kể từ ngày chỉ định
    /// <c>(AllMemberOf = "memberOf:1.2.840.113556.1.4.1941:";)</c>
    /// </summary>
    /// <param name="whenChanged">
    /// Ngày để lọc các nhóm theo ngày thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>Danh sách các đối tượng <see cref="GroupLdap"/> đại diện cho các nhóm LDAP.</returns>
    public async Task<List<GroupLdap>> GetAllGroupsLdapAsync(DateTime whenChanged = default)
    {
        AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
        attributeWithValueCollection.Add(AttributeTypeLdap.AllMemberOf, [_ldapConnectionInfo.RootGroupDn]);
        if (whenChanged != default && whenChanged != DateTimeOffset.MinValue)
        {
            whenChanged = whenChanged.AddSeconds(1);
            attributeWithValueCollection.Add(AttributeTypeLdap.WhenChanged,
                [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThanOrEqual);
        }

        ResultEntryCollectionLdap resultEntryCollectionLdap = new(ldapContext);
        var entryCollection =
            await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, _attributesToReturns);
        return await GroupLdapFromSearchResultEntryCollection(entryCollection);
    }

    /// <summary>
    /// Lấy tất cả các nhóm LDAP đã thay đổi kể từ ngày chỉ định, tìm theo từng tầng dựa trên memberOf.
    /// </summary>
    /// <param name="whenChanged">
    /// Ngày để lọc các nhóm theo ngày thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>Danh sách các đối tượng <see cref="GroupLdap"/> đại diện cho các nhóm LDAP.</returns>
    public async Task<List<GroupLdap>> GetAllGroupsLdapByRecursiveAsync(DateTime whenChanged = default)
    {
        // Danh sách để lưu các nhóm LDAP tìm được
        List<GroupLdap> allGroups = [];
        Queue<string> groupQueue = new(); // Hàng đợi để duyệt theo tầng
        groupQueue.Enqueue(_ldapConnectionInfo.RootGroupDn); // Thêm nhóm gốc vào hàng đợi

        // Nếu có whenChanged, tăng 1s để tránh sai số
        if (whenChanged != default && whenChanged != DateTime.MinValue)
        {
            whenChanged = whenChanged.AddSeconds(1);
        }

        while (groupQueue.Count > 0)
        {
            // Lấy nhóm hiện tại từ hàng đợi
            var currentGroupDn = groupQueue.Dequeue();

            // Tạo bộ lọc LDAP để tìm các nhóm con
            AttributeWithValueCollectionLdap attributeWithValueCollection = new(ObjectClassTypeLdap.Group);
            attributeWithValueCollection.Add(AttributeTypeLdap.MemberOf,
                [currentGroupDn]); // Tìm nhóm con của nhóm hiện tại

            if (whenChanged != default)
            {
                attributeWithValueCollection.Add(AttributeTypeLdap.WhenChanged,
                    [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThanOrEqual);
            }

            // Gửi yêu cầu tìm kiếm
            ResultEntryCollectionLdap resultEntryCollectionLdap = new(ldapContext);
            var entryCollection =
                await resultEntryCollectionLdap.GetAsync(attributeWithValueCollection, _attributesToReturns);

            // Chuyển đổi các kết quả thành danh sách GroupLdap
            var groups = await GroupLdapFromSearchResultEntryCollection(entryCollection);

            // Thêm vào danh sách kết quả
            allGroups.AddRange(groups);

            // Thêm các nhóm vừa tìm được vào hàng đợi để xử lý đệ quy
            foreach (var group in groups)
            {
                if (group.DistinguishedName != null)
                    groupQueue.Enqueue(group.DistinguishedName); // Thêm DN của nhóm con vào hàng đợi
            }
        }

        return allGroups;
    }
}