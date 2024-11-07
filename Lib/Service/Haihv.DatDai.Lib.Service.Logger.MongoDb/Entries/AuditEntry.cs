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
    /// Tên bảng trong cơ sở dữ liệu.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}