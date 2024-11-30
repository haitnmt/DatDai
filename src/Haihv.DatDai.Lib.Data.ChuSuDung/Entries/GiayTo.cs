using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entries;

/// <summary>
/// Lớp trừu tượng đại diện cho giấy tờ của chủ sử dụng.
/// </summary>
/// <param name="soGiayTo">Số giấy tờ.</param>
public abstract class GiayTo(string soGiayTo) : BaseDto, IGiayTo
{
    /// <summary>
    /// Loại giấy tờ.
    /// </summary>
    public virtual int LoaiGiayTo { get; set; }
    
    /// <summary>
    /// Số giấy tờ.
    /// </summary>
    [Column("SoGiayTo",TypeName = "varchar(150)")]
    [JsonPropertyName("soGiayTo")]
    public string SoGiayTo { get; set; } = soGiayTo;
    
    /// <summary>
    /// Ngày cấp giấy tờ.
    /// </summary>
    [Column("NgayCap", TypeName = "date")]
    public DateTime NgayCap { get; set; }
    
    /// <summary>
    /// Nơi cấp giấy tờ.
    /// </summary>
    [Column("NoiCap",TypeName = "varchar(150)")]
    [JsonPropertyName("noiCap")]
    public string NoiCap { get; set; } = string.Empty;
    
    /// <summary>
    /// Mã định danh của giấy tờ.
    /// </summary>
    public virtual string? MaDinhDanh { get; set; }
    
    /// <summary>
    /// Hình thức xác thực giấy tờ.
    /// </summary>
    /// <remarks>
    /// 1: là trạng thái xác thực qua giấy tờ tùy thân
    /// 2: là trạng thái đã xác thực qua VNID
    /// 3: là hình thức xác thực khác
    /// </remarks>
    [Column("HinhThucXacThuc",TypeName = "integer")]
    [JsonPropertyName("hinhThucXacThuc")]
    public int HinhThucXacThuc { get; set; }
    
    /// <summary>
    /// Phiên bản của giấy tờ.
    /// </summary>
    [JsonPropertyName("phienBan")]
    public virtual int PhienBan { get; set; }
}