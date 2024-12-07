using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entities;

/// <summary>
/// Lớp VoChong kế thừa từ ChuSuDungBase, đại diện cho thông tin của vợ chồng.
/// </summary>
public class VoChong : ChuSuDungBase
{
    /// <summary>
    /// Thuộc tính Vo, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của vợ.
    /// </summary>
    [Column("Vo", TypeName = "uuid")]
    [JsonPropertyName("vo")]
    public Guid Vo { get; set; } = Guid.Empty;

    /// <summary>
    /// Thuộc tính Chong, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của chồng.
    /// </summary>
    [Column("Chong", TypeName = "uuid")]
    [JsonPropertyName("chong")]
    public Guid Chong { get; set; } = Guid.Empty;
}