namespace Haihv.DatDai.Data.Base;

public interface IBaseDto
{
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