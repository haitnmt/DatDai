using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;

public static class PostgreSqlExtensions
{
    private static string GetPostgreSqlConnectionString(this IConfigurationManager configurationManager,
        string sectionName = "PostgreSQL", string hostKey = "PrimaryHost", string databaseKey = "Database",
        string userKey = "Username", string passwordKey = "Password")
    {
        var config = configurationManager.GetSection(sectionName);
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

    private static string GetPostgreSqlConnectionString(this IHostApplicationBuilder builder,
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

    /// <summary>
    /// Thêm <see cref="PostgreSqlConnection"/> đăng ký dưới dạng singleton.
    /// </summary>
    /// <param name="builder"><see cref="IHostApplicationBuilder"/> để cấu hình ứng dụng.</param>
    /// <param name="sectionName">Tên của phần cấu hình chứa thông tin kết nối PostgreSQL. Mặc định là "PostgreSQL".</param>
    /// <param name="primaryHostKey"> Khóa của máy chủ PostgreSQL. Mặc định là "PrimaryHost".</param>
    /// <param name="replicaHostKey"> Khóa của máy chủ sao chép PostgreSQL. Mặc định là "ReplicaHost".</param>
    /// <param name="databaseKey">Khóa của cơ sở dữ liệu PostgreSQL. Mặc định là "Database".</param>
    /// <param name="userKey">Khóa của tên người dùng PostgreSQL. Mặc định là "Username".</param>
    /// <param name="passwordKey">Khóa của mật khẩu PostgreSQL. Mặc định là "Password".</param>
    public static void AddPostgreSqlConnection(this IHostApplicationBuilder builder,
        string sectionName = "PostgreSQL", string primaryHostKey = "PrimaryHost", string replicaHostKey = "ReplicaHost",
        string databaseKey = "Database", string userKey = "Username", string passwordKey = "Password")
    {
        var primaryConnectionString =
            GetPostgreSqlConnectionString(builder, sectionName, primaryHostKey, databaseKey, userKey, passwordKey);
        var replicaConnectionString =
            GetPostgreSqlConnectionString(builder, sectionName, replicaHostKey, databaseKey, userKey, passwordKey);
        var postgreSqlConnection = new PostgreSqlConnection
        {
            PrimaryConnectionString = primaryConnectionString,
            ReplicaConnectionString = string.IsNullOrWhiteSpace(replicaConnectionString)
                ? primaryConnectionString
                : replicaConnectionString,
        };
        builder.Services.AddSingleton(postgreSqlConnection);
    }
}

public class PostgreSqlConnection
{
    public string PrimaryConnectionString { get; init; } = string.Empty;
    public string ReplicaConnectionString { get; init; } = string.Empty;
}