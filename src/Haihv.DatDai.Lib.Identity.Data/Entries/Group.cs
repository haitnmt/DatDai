using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

[PrimaryKey("Id")]
public class Group : SoftDeletable
{
    /// <summary>
    /// GUID của nhóm.
    /// </summary>
    [JsonPropertyName("id")]
    [Column("Id", TypeName = "uuid")]
    public Guid Id { get; init; } = Guid.CreateVersion7();
    /// <summary>
    /// Tên chung (Common Name) của nhóm.
    /// </summary>
    [JsonPropertyName("groupName")]
    [Column("GroupName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? GroupName { get; set; }

    /// <summary>
    /// Tập hợp các nhóm mà nhóm này là thành viên.
    /// </summary>
    [JsonPropertyName("memberOf")]
    [Column("MemberOf", TypeName = "uuid[]")]
    public HashSet<Guid> MemberOf { get; set; } = [];

    /// <summary>
    /// Mã hash của nhóm.
    /// </summary>
    [JsonPropertyName("hashGroup")]
    [Column("HashGroup", TypeName = "varchar(64)")]
    [MaxLength(64)]
    public string? HashGroup { get; set; }
    /// <summary>
    /// Những vẫn đề cần chú ý về nhóm.
    /// </summary>
    [JsonPropertyName("ghiChu")]
    [Column("GhiChu", TypeName = "varchar(250)")]
    [MaxLength(250)]
    public string? GhiChu { get; set; }
    
    /// <summary>
    /// Thời gian tạo nhóm.
    /// </summary>
    [JsonPropertyName("whenCreated")]
    [Column("WhenCreated", TypeName = "timestamp with time zone")]
    public DateTimeOffset WhenCreated { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Thời gian thay đổi nhóm lần cuối.
    /// </summary>
    [JsonPropertyName("whenChanged")]
    [Column("WhenChanged", TypeName = "timestamp with time zone")]
    public DateTimeOffset? WhenChanged { get; set; } = DateTimeOffset.UtcNow;
}