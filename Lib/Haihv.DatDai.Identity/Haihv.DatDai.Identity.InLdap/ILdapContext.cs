using System.DirectoryServices.Protocols;

namespace Haihv.DatDai.Identity.InLdap;

public interface ILdapContext
{
    LdapConnectionInfo LdapConnectionInfo { get; }
    LdapConnection Connection { get; }
}