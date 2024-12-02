using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haihv.DatDai.Lib.Data.Base;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

[PrimaryKey("Id")]
public class Group : SoftDeletable
{
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [Column("Id", TypeName = "uuid")]
    public Guid Id { get; init; } = Guid.CreateVersion7();
    /// <summary>
    /// Tên chung (Common Name) của nhóm.
    /// </summary>
    [Column("GroupName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? GroupName { get; set; }

    /// <summary>
    /// Tập hợp các nhóm mà nhóm này là thành viên.
    /// </summary>
    [Column("MemberOf", TypeName = "uuid[]")]
    public HashSet<Guid> MemberOf { get; set; } = [];

    /// <summary>
    /// Những vẫn đề cần chú ý về nhóm.
    /// </summary>
    [Column("GhiChu", TypeName = "varchar(250)")]
    [MaxLength(250)]
    public string? GhiChu { get; set; }
    
    /// <summary>
    /// Thời gian tạo nhóm.
    /// </summary>
    [Column("WhenCreated", TypeName = "timestamp with time zone")]
    public DateTimeOffset WhenCreated { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Thời gian thay đổi nhóm lần cuối.
    /// </summary>
    [Column("WhenChanged", TypeName = "timestamp with time zone")]
    public DateTimeOffset? WhenChanged { get; set; } = DateTimeOffset.UtcNow;
}