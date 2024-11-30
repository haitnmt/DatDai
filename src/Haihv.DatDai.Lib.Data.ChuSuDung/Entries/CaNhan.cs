using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entries;

/// <summary>
/// Lớp đại diện cho cá nhân sử dụng đất.
/// </summary>
public class CaNhan : ChuSuDungBase
{
    /// <summary>
    /// Là họ và tên của người ghi trong giấy tờ nhân thân
    /// </summary>
    [Column("HoTen",TypeName = "varchar(150)")]
    [JsonPropertyName("hoTen")]
    public string? HoTen { get; set; } = string.Empty;

    /// <summary>
    /// Là ngày, tháng, năm sinh (nếu có) của cá nhân.
    /// </summary>
    [Column("NgaySinh",TypeName = "date")]
    [JsonPropertyName("ngaySinh")]
    public DateTime? NgaySinh { get; set; }

    /// <summary>
    /// Năm sinh của cá nhân.
    /// </summary>
    [Column("NamSinh",TypeName = "int")]
    [JsonPropertyName("namSinh")]
    public int NamSinh { get; set; }

    /// <summary>
    /// Giới tính của cá nhân.
    /// </summary>
    /// <remarks>
    /// 0: là nữ,
    /// 1: là nam,
    /// 2: là giới tính khác)
    /// </remarks>
    [Column("GioiTinh",TypeName = "int")]
    [JsonPropertyName("gioiTinh")]
    public int GioiTinh { get; set; }

    /// <summary>
    /// Mã số thuế của cá nhân.
    /// </summary>
    [Column("MaSoThue",TypeName = "varchar(50)")]
    [JsonPropertyName("maSoThue")]
    public string? MaSoThue { get; set; }
    
    /// <summary>
    /// Danh sách ID giấy tờ tùy thân của cá nhân.
    /// </summary>
    [Column("GiayToTuyThanIds",TypeName = "uuid[]")]
    [JsonPropertyName("giayToTuyThanIds")]
    public Guid[]? GiayToTuyThanIds { get; set; }
    
    /// <summary>
    /// ID quốc tịch của cá nhân.
    /// </summary>
    [Column("QuocTichId",TypeName = "uuid")]
    [JsonPropertyName("quocTichId")]
    public Guid? QuocTichId { get; set; }
    
    /// <summary>
    /// ID dân tộc của cá nhân.
    /// </summary>
    [Column("DanTocId",TypeName = "uuid")]
    [JsonPropertyName("danTocId")]
    public Guid? DanTocId { get; set; }
    
}