using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Enum;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public class UserLdapService(ILdapContext ldapContext)
{
    private readonly LdapConnection _ldapConnection = ldapContext.Connection;
    private readonly LdapConnectionInfo _ldapConnectionInfo = ldapContext.LdapConnectionInfo;
    private readonly GroupLdapService _groupLdapService = new(ldapContext);
    private readonly AttributeTypeLdap[] _attributesToReturns = [AttributeTypeLdap.ObjectGuid,
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
        AttributeTypeLdap.AccountExpires];

}