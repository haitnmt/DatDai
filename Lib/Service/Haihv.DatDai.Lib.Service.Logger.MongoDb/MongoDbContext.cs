using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using MongoDB.Driver;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb;

/// <summary>
/// Lớp MongoDbContext cung cấp kết nối và truy cập vào cơ sở dữ liệu MongoDB.
/// </summary>
/// <param name="connectionString">Chuỗi kết nối đến MongoDB.</param>
/// <param name="databaseName">Tên cơ sở dữ liệu MongoDB.</param>
public class MongoDbContext(string connectionString, string databaseName) : IMongoDbContext
{
    /// <summary>
    /// Truy cập vào cơ sở dữ liệu MongoDB.
    /// </summary>
    public IMongoDatabase Database => Constructor(connectionString, databaseName);

    /// <summary>
    /// Truy cập vào bộ sưu tập AuditEntries.
    /// </summary>
    public IMongoCollection<AuditEntry> AuditEntries => Database.GetCollection<AuditEntry>("AuditEntries");

    /// <summary>
    /// Truy cập vào bộ sưu tập LogSystemEntries.
    /// </summary>
    public IMongoCollection<LogSystemEntry> LogSystemEntries => Database.GetCollection<LogSystemEntry>("LogSystemEntries");

    /// <summary>
    /// Truy cập vào bộ sưu tập logUserEntries.
    /// </summary>
    public IMongoCollection<LogUserEntry> LogUserEntries => Database.GetCollection<LogUserEntry>("LogUserEntries");

    /// <summary>
    /// Hàm khởi tạo cơ sở dữ liệu MongoDB.
    /// </summary>
    /// <param name="connectionString">Chuỗi kết nối đến MongoDB.</param>
    /// <param name="databaseName">Tên cơ sở dữ liệu MongoDB.</param>
    /// <returns>Trả về đối tượng IMongoDatabase.</returns>
    /// <exception cref="ArgumentNullException">Ném ngoại lệ nếu connectionString hoặc databaseName là null hoặc rỗng.</exception>
    private static IMongoDatabase Constructor(string? connectionString, string? databaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentNullException(nameof(databaseName));
        }
        
        var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
        if (connectionString.Contains("mongodb+srv://"))
        {
            // Kết nối Atlas - Đặt Server API thành V1 để tương thích tốt nhất
            mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        }

        // Tạo MongoClient một lần và tái sử dụng để đạt hiệu quả
        var mongoClient = new MongoClient(mongoClientSettings);

        // Lấy tham chiếu cơ sở dữ liệu
        return mongoClient.GetDatabase(databaseName);
    }
}
