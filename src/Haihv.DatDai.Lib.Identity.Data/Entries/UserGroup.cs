using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Haihv.DatDai.Lib.Data.Base;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;
[PrimaryKey("Id")]
public class UserGroup : BaseEntry
{
    /// <summary>
    /// Id của UserGroup.
    /// </summary>
    [Column("Id", TypeName = "uuid")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    [Column("UserId", TypeName = "uuid")]
    public Guid UserId { get; set; } = Guid.Empty;
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [Column("GroupId", TypeName = "uuid")]
    public Guid GroupId { get; set; } = Guid.Empty;
    /// <summary>
    /// Những vấn đề cần lưu ý
    /// </summary>
    [Column("GhiChu", TypeName = "varchar(250)")]
    [MaxLength(250)]
    public string? GhiChu { get; set; }
}