namespace Haihv.DatDai.Lib.Identity.Data.Entries;

/// <summary>
/// Lớp trừu tượng đại diện cho đăng ký vai trò.
/// </summary>
public abstract class RoleRegistration
{
    /// <summary>
    /// Mã định danh duy nhất cho đăng ký vai trò.
    /// </summary>
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Mã định danh của người dùng (có thể null).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Mã định danh của nhóm (có thể null).
    /// </summary>
    public Guid? GroupId { get; set; }
    
    /// <summary>
    /// Trạng thái vô hiệu hóa của đăng ký vai trò.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Ghi chú bổ sung (có thể null).
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Thời điểm tạo đăng ký vai trò.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Thời điểm cập nhật đăng ký vai trò (có thể null).
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}