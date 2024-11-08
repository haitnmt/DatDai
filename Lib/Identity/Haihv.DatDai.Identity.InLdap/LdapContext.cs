﻿using System.DirectoryServices.Protocols;
using System.Net;

namespace Haihv.DatDai.Identity.InLdap;

public class LdapContext(LdapConnectionInfo ldapConnectionInfo) : ILdapContext
{
    public LdapConnectionInfo LdapConnectionInfo => ldapConnectionInfo;

    public LdapConnection Connection
        => CreateConnection();

    private LdapConnection CreateConnection()
    {
            LdapDirectoryIdentifier ldapDirectoryIdentifier = new(LdapConnectionInfo.Host, LdapConnectionInfo.Port, true, false);
            LdapConnection ldapConnection = new(ldapDirectoryIdentifier,
                new NetworkCredential(LdapConnectionInfo.Username, LdapConnectionInfo.Password))
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
    public string Username { get; init; } = "Administrator";
    public string Password { get; init; } = string.Empty;
    public string RootGroupDn { get; init; } = "cn=Administrators";
    public string AdminGroupDn { get; init; } = "cn=Administrators";
    public string SearchBase { get; init; } = "dc=example,dc=com";
    public string Domain { get; init; } = "example";
    public string DomainFullname { get; init; } = "example.com";
    public string Organizational { get; init; } = "ou=Users";
}