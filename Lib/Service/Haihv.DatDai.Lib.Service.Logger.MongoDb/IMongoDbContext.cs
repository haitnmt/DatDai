using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using MongoDB.Driver;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb;

/// <summary>
/// Giao diện cho MongoDB context.
/// </summary>
public interface IMongoDbContext
{
    /// <summary>
    /// Lấy đối tượng cơ sở dữ liệu MongoDB.
    /// </summary>
    IMongoDatabase Database { get; }

    /// <summary>
    /// Lấy bộ sưu tập người dùng.
    /// </summary>
    IMongoCollection<AuditEntry> AuditEntries { get; }

    /// <summary>
    /// Lấy bộ sưu tập nhóm hệ thống.
    /// </summary>
    IMongoCollection<LogSystemEntry> LogSystemEntries { get; }

    /// <summary>
    /// Lấy bộ sưu tập nhóm người dùng.
    /// </summary>
    IMongoCollection<LogUserEntry> LogUserEntries { get; }
}