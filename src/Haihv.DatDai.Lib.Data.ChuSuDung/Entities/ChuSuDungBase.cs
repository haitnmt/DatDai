using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;
using Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entities;

public abstract class ChuSuDungBase: BaseDto, IChuSuDungDatBase
{
    /// <summary>
    /// ID của địa chỉ.
    /// </summary>
    [Column("DiaChiId", TypeName = "uuid")]
    public Guid DiaChiId { get; set; } = Guid.Empty;
    /// <summary>
    /// Phiên bản của chủ sử dụng đất.
    /// </summary>
    [Column("PhienBan",TypeName = "integer")]
    [JsonPropertyName("phienBan")]
    public int PhienBan { get; set; }
    /// <summary>
    /// Hiệu lực của chủ sử dụng đất.
    /// </summary>
    /// <remarks>
    /// Là hiệu lực để xác định phiên bản là hiệu lực mới nhất,
    /// các phiên bản thấp hơn là không còn hiệu lực vì đã bị thay đổi thông tin
    /// </remarks>
    [Column("HieuLuc",TypeName = "boolean")]
    [JsonPropertyName("hieuLuc")]
    public bool HieuLuc { get; set; }
}