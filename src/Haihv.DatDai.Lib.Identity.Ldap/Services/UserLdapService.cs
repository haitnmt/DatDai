using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Entities;
using Haihv.DatDai.Lib.Identity.Ldap.Enum;
using Haihv.DatDai.Lib.Identity.Ldap.Extension;
using Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public sealed class UserLdapService(ILdapContext ldapContext) : IUserLdapService
{
    private readonly LdapConnectionInfo _ldapConnectionInfo = ldapContext.LdapConnectionInfo;
    
    private readonly AttributeTypeLdap[] _attributesToReturns =
    [
        AttributeTypeLdap.ObjectGuid,
        AttributeTypeLdap.UserPrincipalName,
        AttributeTypeLdap.DisplayName,
        AttributeTypeLdap.DistinguishedName,
        AttributeTypeLdap.SamAccountName,
        AttributeTypeLdap.Cn,
        AttributeTypeLdap.Mail,
        AttributeTypeLdap.MemberOf,
        AttributeTypeLdap.JobTitle,
        AttributeTypeLdap.Department,
        AttributeTypeLdap.Description,
        AttributeTypeLdap.UserAccountControl,
        AttributeTypeLdap.PwdLastSet,
        AttributeTypeLdap.LockoutTime,
        AttributeTypeLdap.AccountExpires,
        AttributeTypeLdap.WhenCreated,
        AttributeTypeLdap.WhenChanged
    ];

    /// <summary>
    /// Lấy thông tin người dùng từ LDAP.
    /// </summary>
    /// <param name="userPrincipalName">Tên người dùng cần lấy thông tin.</param>
    /// <param name="whenChanged"></param>
    /// <returns>Đối tượng UserLdap chứa thông tin người dùng.</returns>
    public async Task<UserLdap?> GetByPrincipalNameAsync(string userPrincipalName, DateTime whenChanged = default)
    {
        AttributeWithValueCollectionLdap filterCollection = new();
        filterCollection.Add(AttributeTypeLdap.UserPrincipalName, [userPrincipalName]);
        // Thêm điều kiện lọc theo ngày thay đổi cuối cùng của nhóm
        if (whenChanged != default && whenChanged != DateTime.MinValue)
            filterCollection.Add(AttributeTypeLdap.WhenChanged,
                [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThanOrEqual);
        
        var resultLdap = new ResultEntryCollectionLdap(ldapContext);
        var resultEntries = await resultLdap.GetAsync(filterCollection, _attributesToReturns);
        if (resultEntries is null || resultEntries.Count <= 0) return null;
        return UserLdapFromSearchResultEntryCollection(resultEntries)[0];
    }
    
    /// <summary>
    /// Lấy thông tin người dùng từ LDAP.
    /// </summary>
    /// <param name="distinguishedName">
    /// Tên định danh của người dùng cần lấy thông tin.
    /// </param>
    /// <param name="whenChanged"></param>
    /// <returns>Đối tượng UserLdap chứa thông tin người dùng.</returns>
    public async Task<UserLdap?> GetByDistinctNameAsync(string distinguishedName, DateTime whenChanged = default)
    {
        AttributeWithValueCollectionLdap attributeWithValueCollection = new();
        attributeWithValueCollection.Add(AttributeTypeLdap.DistinguishedName, [distinguishedName]);
        // Thêm điều kiện lọc theo ngày thay đổi cuối cùng của nhóm
        if (whenChanged != default && whenChanged != DateTime.MinValue)
            attributeWithValueCollection.Add(AttributeTypeLdap.WhenChanged,
                [whenChanged.ToString("yyyyMMddHHmmss.0Z")], OperatorLdap.GreaterThanOrEqual);
        var resultLdap = new ResultEntryCollectionLdap(ldapContext);
        var resultEntries = await resultLdap.GetAsync(attributeWithValueCollection, _attributesToReturns);
        if (resultEntries is null || resultEntries.Count <= 0) return null;
        return UserLdapFromSearchResultEntryCollection(resultEntries)[0];
    }
    /// <summary>
    /// Chuyển đổi một tập hợp các kết quả tìm kiếm LDAP thành danh sách các đối tượng UserLdap.
    /// </summary>
    /// <param name="resultEntries">Tập hợp các kết quả tìm kiếm LDAP.</param>
    /// <param name="isGetAll">Xác định có lấy tất cả các kết quả hay không.</param>
    /// <returns>Danh sách các đối tượng UserLdap.</returns>
    private List<UserLdap> UserLdapFromSearchResultEntryCollection(SearchResultEntryCollection? resultEntries,
        bool isGetAll = true)
    {
        List<UserLdap> result = [];
        if (resultEntries is null || resultEntries.Count <= 0) return result;
        foreach (SearchResultEntry entry in resultEntries)
        {
            var intFileTimeUtc = DateTime.UtcNow.ToFileTimeUtc();
            var pwdLastSet = DateTime.MinValue;
            if (long.TryParse(entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.PwdLastSet)]?[0].ToString(),
                    out var pwdLastSetRaw))
            {
                pwdLastSet = DateTime.FromFileTimeUtc(pwdLastSetRaw);
                // Làm tròn xuống đến giây gần nhất
                pwdLastSet = pwdLastSet.AddTicks(-(pwdLastSet.Ticks % TimeSpan.TicksPerSecond));
            }

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
            
            UserLdap detailUser = new()
            {
                Id = new Guid((byte[])entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.ObjectGuid)][0]),
                DisplayName =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.DisplayName)]?[0].ToString() ??
                    entry.Attributes["cn"]?[0].ToString() ?? string.Empty,
                UserPrincipalName =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.UserPrincipalName)]?[0].ToString() ??
                    string.Empty,
                DistinguishedName =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.DistinguishedName)]?[0].ToString() ??
                    string.Empty,
                Email = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Mail)]?[0].ToString() ??
                        string.Empty,
                Department =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Department)]?[0].ToString() ??
                    string.Empty,
                JobTitle = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.JobTitle)]?[0].ToString() ??
                           string.Empty,
                Description =
                    entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.Description)]?[0].ToString() ??
                    string.Empty,
                IsLocked = (int.TryParse(
                                entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.UserAccountControl)]?[0]
                                    .ToString(), out var userAccountControl)
                            && Convert.ToBoolean(userAccountControl & 0x002))
                           || (long.TryParse(
                                   entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.LockoutTime)]?[0]
                                       .ToString(), out var lockoutTime)
                               && lockoutTime > intFileTimeUtc)
                           || (long.TryParse(
                                   entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.AccountExpires)]?[0]
                                       .ToString(), out var accountExpires)
                               && accountExpires > 0 && accountExpires < intFileTimeUtc),
                PwdLastSet = pwdLastSet,
                IsPwdMustChange = pwdLastSetRaw == 0,
                MemberOf = entry.Attributes[AttributeLdap.GetAttribute(AttributeTypeLdap.MemberOf)]?.GetValues(typeof(string)).Cast<string>().ToHashSet()?? [],
                Organization = _ldapConnectionInfo.Organizational,
                WhenCreated = whenCreated,
                WhenChanged = whenChanged
            };
            result.Add(detailUser);
            if (!isGetAll) break;
        }

        return result;
    }
}