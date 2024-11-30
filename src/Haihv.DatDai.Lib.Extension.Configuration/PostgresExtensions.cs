using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Extension.Configuration;

public static class PostgresExtensions
{
    private static string GetPostgreSqlConnectionString(this IConfigurationManager configuration,
        string sectionName = "PostgreSQL", string hostKey = "PrimaryHost", string databaseKey = "Database",
        string userKey = "Username", string passwordKey = "Password")
    {
        var config = configuration.GetSection(sectionName);
        var host = config[hostKey]?.Trim();
        if (string.IsNullOrWhiteSpace(host)) return string.Empty;
        var database = config[databaseKey]?.Trim();
        if (string.IsNullOrWhiteSpace(database)) return string.Empty;
        var username = config[userKey]?.Trim();
        if (string.IsNullOrWhiteSpace(username)) return string.Empty;
        var password = config[passwordKey]?.Trim();
        return
            $"Host={host}; Database={database}; Username={username}; {(string.IsNullOrWhiteSpace(password) ? string.Empty : $"Password={password}")}";
    }

    public static string GetPostgreSqlConnectionString(this IHostApplicationBuilder builder,
        string sectionName, string hostKey, string databaseKey = "Database",
        string userKey = "Username", string passwordKey = "Password")
    {
        return GetPostgreSqlConnectionString(builder.Configuration, sectionName, hostKey, databaseKey, userKey,
            passwordKey);
    }

    public static PostgreSqlConnection GetPostgreSqlConnectionString(this IHostApplicationBuilder builder,
        string sectionName = "PostgreSQL")
    {
        var primaryConnectionString = GetPostgreSqlConnectionString(builder, sectionName, hostKey: "PrimaryHost");
        var replicaConnectionString = GetPostgreSqlConnectionString(builder, sectionName, hostKey: "ReplicaHost");
        return new PostgreSqlConnection
        {
            PrimaryConnectionString = primaryConnectionString,
            ReplicaConnectionString = string.IsNullOrWhiteSpace(replicaConnectionString)
                ? primaryConnectionString
                : replicaConnectionString,
        };
    }
}

public class PostgreSqlConnection
{
    public string PrimaryConnectionString { get; init; } = string.Empty;
    public string ReplicaConnectionString { get; init; } = string.Empty;
}