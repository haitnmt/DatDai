namespace Haihv.DatDai.Lib.Data.Base;

/// <summary>
/// Lớp cơ sở cho các thực thể có thể xóa mềm.
/// </summary>
public abstract class SoftDeletable : ISoftDeletable
{
    /// <summary>
    /// Xác định thực thể có bị xóa mềm hay không.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Thời gian thực thể bị xóa mềm (UTC).
    /// </summary>
    public DateTimeOffset? DeletedAtUtc { get; set; }
}