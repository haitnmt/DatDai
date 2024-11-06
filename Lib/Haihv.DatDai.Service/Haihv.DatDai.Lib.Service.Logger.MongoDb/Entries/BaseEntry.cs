using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;

/// <summary>
/// Lớp cơ sở cho các mô hình trong MongoDB.
/// </summary>
public abstract class BaseEntry : IBaseEntry
{
    /// <summary>
    /// Id của đối tượng.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    [JsonPropertyName("id")]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Thông tin metadata.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("metadata")]
    [JsonPropertyName("metadata")]
    public string? Metadata { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian bắt đầu (UTC).
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonIgnoreIfDefault]
    [BsonElement("startTimeUtc")]
    [JsonPropertyName("startTimeUtc")]
    public DateTime StartTimeUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian kết thúc (UTC).
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("endTimeUtc")]
    [JsonPropertyName("endTimeUtc")]
    public DateTime? EndTimeUtc { get; set; }

    /// <summary>
    /// Trạng thái thành công.
    /// </summary>
    [BsonRepresentation(BsonType.Boolean)]
    [BsonIgnoreIfDefault]
    [BsonElement("success")]
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Thông tin ngoại lệ nếu có.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("exception")]
    [JsonPropertyName("exception")]
    public string? Exception { get; set; }
}