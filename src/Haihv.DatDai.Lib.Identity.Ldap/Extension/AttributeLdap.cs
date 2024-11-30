using Haihv.DatDai.Lib.Identity.Ldap.Enum;

namespace Haihv.DatDai.Lib.Identity.Ldap.Extension;

public abstract class AttributeLdap(AttributeTypeLdap attributeType)
{
    private const string ObjectGuid = "objectGUID";
    private const string ObjectClass = "objectClass";
    private const string DistinguishedName = "distinguishedName";
    private const string SamAccountName = "sAMAccountName";
    private const string UserPrincipalName = "userPrincipalName";
    private const string DisplayName = "displayName";
    private const string Cn = "cn";
    private const string Mail = "mail";
    private const string MemberOf = "memberOf";
    private const string Member = "member";
    private const string Description = "description";
    private const string JobTitle = "title";
    private const string Department = "department";
    private const string PwdLastSet = "pwdLastSet";
    private const string AccountExpires = "accountExpires";
    private const string LockoutTime = "lockoutTime";
    private const string UserAccountControl = "userAccountControl";
    private const string WhenChanged = "whenChanged";
    private const string AllMember = "member:1.2.840.113556.1.4.1941:";
    private const string AllMemberOf = "memberOf:1.2.840.113556.1.4.1941:";
    public string Name => GetAttribute(attributeType);

    public static string GetAttribute(AttributeTypeLdap attributeType)
    {
        return attributeType switch
        {
            AttributeTypeLdap.ObjectGuid => ObjectGuid,
            AttributeTypeLdap.ObjectClass => ObjectClass,
            AttributeTypeLdap.DistinguishedName => DistinguishedName,
            AttributeTypeLdap.SamAccountName => SamAccountName,
            AttributeTypeLdap.UserPrincipalName => UserPrincipalName,
            AttributeTypeLdap.DisplayName => DisplayName,
            AttributeTypeLdap.Cn => Cn,
            AttributeTypeLdap.Mail => Mail,
            AttributeTypeLdap.MemberOf => MemberOf,
            AttributeTypeLdap.Member => Member,
            AttributeTypeLdap.Description => Description,
            AttributeTypeLdap.JobTitle => JobTitle,
            AttributeTypeLdap.Department => Department,
            AttributeTypeLdap.PwdLastSet => PwdLastSet,
            AttributeTypeLdap.AccountExpires => AccountExpires,
            AttributeTypeLdap.LockoutTime => LockoutTime,
            AttributeTypeLdap.UserAccountControl => UserAccountControl,
            AttributeTypeLdap.WhenChanged => WhenChanged,
            AttributeTypeLdap.AllMember => AllMember,
            AttributeTypeLdap.AllMemberOf => AllMemberOf,
            _ => string.Empty,
        };
    }
    public static string[] GetAttributes(AttributeTypeLdap[] attributeTypes)
    {
        return attributeTypes.Select(GetAttribute).ToArray();
    }
}