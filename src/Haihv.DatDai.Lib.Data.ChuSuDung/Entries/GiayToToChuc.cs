using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entries;

/// <summary>
/// Lớp đại diện cho giấy tờ tổ chức.
/// </summary>
/// <param name="soGiayTo">Số giấy tờ tổ chức.</param>
/// <param name="loaiGiayToToChuc">Loại giấy tờ tổ chức (mặc định là 0).</param>
public class GiayToToChuc(string soGiayTo, int loaiGiayToToChuc = 0) : GiayTo(soGiayTo)
{
    /// <summary>
    /// Là loại giấy tờ tổ chức nằm trong bảng mã loại giấy tờ.
    /// </summary>
    [Column("HinhThucXacThuc",TypeName = "integer")]
    [JsonPropertyName("loaiGiayToToChuc")]
    public override int LoaiGiayTo { get; set; } = loaiGiayToToChuc;
    
    /// <summary>
    /// Mã định danh doanh nghiệp.
    /// </summary>
    [Column("MaDinhDanhDoanhNghiep",TypeName = "varchar(150)")]
    [JsonPropertyName("maDinhDanhDoanhNghiep")]
    public override string? MaDinhDanh { get; set; }
    
    /// <summary>
    /// Phiên bản tổ chức.
    /// </summary>
    [Column("PhienBanToChuc",TypeName = "integer")]
    [JsonPropertyName("phienBanToChuc")]
    public override int PhienBan { get; set; }
}