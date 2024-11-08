using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;

/// <summary>
/// Đại diện cho một bản ghi nhật ký kiểm toán.
/// </summary>
/// <remarks>
/// Lưu trữ thông tin nhật ký kiểm toán.
/// </remarks>
/// <seealso cref="BaseEntry"/>
public class AuditEntry : BaseEntry
{
    /// <summary>
    /// Tên của bản ghi.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfDefault]
    [BsonElement("entryName")]
    [JsonPropertyName("entryName")]
    public string EntryName { get; set; } = string.Empty;
}
public static class AuditEntryExtensions
{
    public static string GetHash(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}