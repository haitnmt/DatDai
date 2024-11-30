using System.DirectoryServices.Protocols;

namespace Haihv.DatDai.Lib.Identity.Ldap;

public interface ILdapContext
{
    LdapConnectionInfo LdapConnectionInfo { get; }
    LdapConnection Connection { get; }
}