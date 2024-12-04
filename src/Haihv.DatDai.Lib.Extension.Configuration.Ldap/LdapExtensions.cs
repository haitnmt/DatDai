using Haihv.DatDai.Lib.Identity.Ldap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Extension.Configuration.Ldap;

public static class LdapExtensions
{
    private static LdapConnectionInfo GetLdapConnectionInfo(this IConfigurationManager configurationManager,
        string sectionName = "LDAP", string hostKey = "Host", string portKey = "Port", 
        string domainKey = "Domain", string domainFullNamesKey = "DomainFullName", string organizationalKey = "Organizational",
        string searchBaseKey = "SearchBase", string rootGroupDnKey = "RootGroupDn", string adminGroupDnKey = "AdminGroupDn",
        string adminPrincipalNameKey = "AdminPrincipalName", string adminPasswordKey = "AdminPassword",
        string defaultSyncDelayKey = "DefaultSyncDelay")
    {
        var configurationSection = configurationManager.GetSection(sectionName);
        return new LdapConnectionInfo
        {
            Host = configurationSection[hostKey] ?? string.Empty,
            Port = int.Parse(configurationSection[portKey] ?? "389"),
            Domain = configurationSection[domainKey] ?? string.Empty,
            DomainFullname = configurationSection[domainFullNamesKey] ?? string.Empty,
            Organizational = configurationSection[organizationalKey]?? string.Empty,
            SearchBase = configurationSection[searchBaseKey] ?? string.Empty,
            RootGroupDn = configurationSection[rootGroupDnKey] ?? string.Empty,
            AdminGroupDn = configurationSection[adminGroupDnKey] ?? string.Empty,
            AdminPrincipalName = configurationSection[adminPrincipalNameKey] ?? string.Empty,
            AdminPassword = configurationSection[adminPasswordKey] ?? string.Empty,
            DefaultSyncDelay = int.Parse(configurationSection[defaultSyncDelayKey] ?? "300"),
        };
    }

    private static LdapConnectionInfo GetLdapConnectionInfo(this IHostApplicationBuilder builder,
        string sectionName = "LDAP", string hostKey = "Host", string portKey = "Port", 
        string domainKey = "Domain", string domainFullNamesKey = "DomainFullName", string organizationalKey = "Organizational",
        string searchBaseKey = "SearchBase", string rootGroupDnKey = "RootGroupDn", string adminGroupDnKey = "AdminGroupDn",
        string adminPrincipalNameKey = "AdminPrincipalName", string adminPasswordKey = "AdminPassword",
        string defaultSyncDelayKey = "DefaultSyncDelay")
    {
        return GetLdapConnectionInfo(builder.Configuration, sectionName, hostKey, portKey, domainKey, domainFullNamesKey,
            organizationalKey, searchBaseKey, rootGroupDnKey, adminGroupDnKey, adminPrincipalNameKey, adminPasswordKey,
            defaultSyncDelayKey);
    }

    public static void AddLdapContext(this IHostApplicationBuilder builder,
        string sectionName = "LDAP", string hostKey = "Host", string portKey = "Port",
        string domainKey = "Domain", string domainFullNamesKey = "DomainFullName", string organizationalKey = "Organizational",
        string searchBaseKey = "SearchBase", string rootGroupDnKey = "RootGroupDn", string adminGroupDnKey = "AdminGroupDn",
        string adminPrincipalNameKey = "AdminPrincipalName", string adminPasswordKey = "AdminPassword",
        string defaultSyncDelayKey = "DefaultSyncDelay")
    {
        var ldapConnectionInfo = builder.GetLdapConnectionInfo(sectionName, hostKey, portKey, domainKey, domainFullNamesKey,
            organizationalKey, searchBaseKey, rootGroupDnKey, adminGroupDnKey, adminPrincipalNameKey, adminPasswordKey,
            defaultSyncDelayKey);
        builder.Services.AddSingleton(ldapConnectionInfo);
        builder.Services.AddSingleton<ILdapContext, LdapContext>();
    }
}