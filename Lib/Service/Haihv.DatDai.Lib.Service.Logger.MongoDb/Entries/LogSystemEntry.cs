using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;

/// <summary>
/// Đối tượng mô tả log hệ thống
/// </summary>
/// <remarks>
/// Lưu trữ thông tin log của các hoạt động trong hệ thống
/// </remarks>
/// <inheritdoc/>
/// <seealso cref="BaseEntry"/>
public class LogSystemEntry : BaseEntry
{
    /// <summary>
    /// Tên hệ thống
    /// </summary>
    /// <value>Chuỗi ký tự thể hiện tên của hệ thống</value>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("systemName")]
    [JsonPropertyName("systemName")]
    public string? SystemName { get; set; }

    /// <summary>
    /// Mô tả về hệ thống
    /// </summary>
    /// <value>Chuỗi ký tự mô tả chi tiết về hệ thống</value>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("systemDescription")]
    [JsonPropertyName("systemDescription")]
    public string? SystemDescription { get; set; }

    /// <summary>
    /// Tên chức năng
    /// </summary>
    /// <value>Chuỗi ký tự thể hiện tên của chức năng được thực thi</value>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("functionName")]
    [JsonPropertyName("functionName")]
    public string? FunctionName { get; set; }
}