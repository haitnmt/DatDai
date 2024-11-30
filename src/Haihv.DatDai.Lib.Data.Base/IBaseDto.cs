namespace Haihv.DatDai.Lib.Data.Base;

public interface IBaseDto : ISoftDeletable
{
    /// <summary>
    /// Id của đối tượng.
    /// </summary>
    public Guid Id { get; init; }
    /// <summary>
    /// Ghi chú cho đối tượng.
    /// </summary>
    public string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Thời gian cập nhật đối tượng.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}