using System.DirectoryServices.Protocols;
using System.Net;
using System.Text.Json;

namespace Haihv.DatDai.Lib.Identity.Ldap;

public class LdapContext(LdapConnectionInfo ldapConnectionInfo) : ILdapContext
{
    public LdapConnectionInfo LdapConnectionInfo => ldapConnectionInfo;

    public LdapConnection Connection
        => CreateConnection();

    private LdapConnection CreateConnection()
    {
        LdapDirectoryIdentifier ldapDirectoryIdentifier = new(LdapConnectionInfo.Host, LdapConnectionInfo.Port, true, false);
        LdapConnection ldapConnection = new(ldapDirectoryIdentifier,
            new NetworkCredential(LdapConnectionInfo.AdminPrincipalName, LdapConnectionInfo.AdminPassword))
        {
            AuthType = AuthType.Basic,
            AutoBind = true
        };
        ldapConnection.SessionOptions.ProtocolVersion = 3;
        ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
        return ldapConnection;
    }
}

public class LdapConnectionInfo
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 389;
    public string Domain { get; init; } = "example";
    public string DomainFullname { get; init; } = "example.com";
    public string Organizational { get; init; } = "ou=Users";
    public string SearchBase { get; init; } = "dc=example,dc=com";
    public string RootGroupDn { get; init; } = "cn=Administrators";
    public string AdminGroupDn { get; init; } = "cn=Administrators";
    public string AdminPrincipalName { get; init; } = "Administrator";
    public string AdminPassword { get; init; } = string.Empty;
    public int DefaultSyncDelay { get; init; } = 300; // 5 minutes = 300 seconds
}

public class LogLdapInfo
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 389;
    public string Domain { get; init; } = "example.com";
}

public static class LdapContextExtensions
{
    public static string ToLogInfo(this ILdapContext ldapContext)
    {
        var ldapConnectionInfo = ldapContext.LdapConnectionInfo;
        return JsonSerializer.Serialize(new LogLdapInfo
        {
            Host = ldapConnectionInfo.Host,
            Port = ldapConnectionInfo.Port,
            Domain = ldapConnectionInfo.DomainFullname
        });
    }

    public static string GetUserPrincipalName(this ILdapContext ldapContext, string userName)
    {
        var ldapConnectionInfo = ldapContext.LdapConnectionInfo;
        return $"{(userName.Replace($"{ldapConnectionInfo.Domain}\\", "")).Trim()}@{ldapConnectionInfo.DomainFullname}";
    }

    public static bool CheckUserLdap(this ILdapContext ldapContext, string userName)
    {
        var ldapConnectionInfo = ldapContext.LdapConnectionInfo;
        return userName.StartsWith($"{ldapConnectionInfo.Domain}\\") ||
               userName.EndsWith($"@{ldapConnectionInfo.DomainFullname}") ||
               (!userName.Contains('@') && !userName.Contains('\\'));
    }
}
