using System.Reflection;
using Audit.Core;
using Audit.MongoDB.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Extension.Audit.MongoDb;

public static class AuditToMongoDbExtensions
{
    private const string DefaultDatabase = "DatDai";
    private const string DefaultCollection = "AuditLogs";

    private static AuditDataProvider GetAuditDatToMongoDb(string connectionString, string database, string collection)
    {
        return new MongoDataProvider()
        {
            ConnectionString = connectionString,
            Database = database,
            Collection = collection
        };
    }

    private static AuditDataProvider? GetAuditDatToMongoDb(this IHostApplicationBuilder builder,
        string sectionName = "MongoDb", string connectionStringKey = "ConnectionString",
        string databaseKey = "Database", string collectionKey = "AuditCollection")
    {
        var config = builder.Configuration.GetSection(sectionName);
        var connectionString = config[connectionStringKey]?.Trim();
        if (string.IsNullOrWhiteSpace(connectionString)) return null;
        var database = config[databaseKey]?.Trim();
        database ??= Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Replace(".", "-") ??
                     DefaultDatabase;
        var collection = config[collectionKey]?.Trim();
        collection ??= DefaultCollection;
        return string.IsNullOrWhiteSpace(collection)
            ? null
            : GetAuditDatToMongoDb(connectionString, database, collection);
    }

    /// <summary>
    /// Cấu hình Audit để sử dụng MongoDB.
    /// Thêm <see cref="AuditDataProvider"/> đăng ký dưới dạng singleton.
    /// </summary>
    /// <param name="builder">Đối tượng <see cref="IHostApplicationBuilder"/> để cấu hình.</param>
    /// <param name="sectionName">Tên của section trong cấu hình để lấy thông tin kết nối MongoDB. Mặc định là "MongoDb".</param>
    /// <param name="connectionStringKey">Khóa của chuỗi kết nối trong section cấu hình. Mặc định là "ConnectionString".</param>
    /// <param name="databaseKey">Khóa của tên cơ sở dữ liệu trong section cấu hình. Mặc định là "Database".</param>
    /// <param name="collectionKey">Khóa của tên collection trong section cấu hình. Mặc định là "AuditCollection".</param>
    public static void UseAuditDatToMongoDb(this IHostApplicationBuilder builder,
        string sectionName = "MongoDb", string connectionStringKey = "ConnectionString",
        string databaseKey = "Database", string collectionKey = "AuditCollection")
    {
        var provider = GetAuditDatToMongoDb(builder, sectionName, connectionStringKey, databaseKey, collectionKey);
        if (provider != null)
        {
            builder.Services.AddSingleton(provider);
        }
    }
}