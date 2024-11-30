namespace Haihv.DatDai.Lib.Data.Base;

/// <summary>
/// Giao diện cho các thực thể có thể bị xóa mềm.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Trạng thái xóa mềm của thực thể.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Thời gian thực thể bị xóa mềm (UTC).
    /// </summary>
    public DateTimeOffset? DeletedAtUtc { get; set; }
}