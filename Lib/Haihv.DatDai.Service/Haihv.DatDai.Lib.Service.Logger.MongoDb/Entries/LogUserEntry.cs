using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;

/// <summary>
/// Lớp đại diện cho một mục nhật ký người dùng.
/// </summary>
/// <remarks>
/// Lưu trữ thông tin log của người dùng
/// </remarks>
/// <seealso cref="BaseEntry"/>
public class LogUserEntry : BaseEntry
{
    /// <summary>
    /// ID của người dùng.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("useriD")]
    [JsonPropertyName("userId")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// Tên của người dùng.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    [BsonElement("userName")]
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }
}