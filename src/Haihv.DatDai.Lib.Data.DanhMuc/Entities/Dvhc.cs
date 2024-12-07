using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Entities;

/// <summary>
/// Lớp đại diện cho thông tin về đơn vị hành chính trong cơ sở dữ liệu đất đai.
/// </summary>
public class Dvhc : BaseDto, IDanhMuc
{
    /// 
    /// <summary>
    /// Là mã đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là mã đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maDvhc")]
    public string MaKyHieu
    {
        get
        {
            if (MaXa is > 0)
            {
                return MaXa?.ToString("000000") ?? string.Empty;
            }
            if (MaHuyen > 0)
            {
                return MaHuyen?.ToString("000") ??string.Empty;
            }
            return MaTinh.ToString("00");
        }
    }

    /// <summary>
    /// Là tên đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("TenDvhc",TypeName = "varchar(150)")]
    [JsonPropertyName("tenDvhc")]
    [MaxLength(150)]
    public string TenGiaTri { get; set; } = string.Empty;
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp xã dạng số lưu trong cơ sở dữ liệu.
    /// Xã, phường, thị trấn.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("MaXa",TypeName = "integer")]
    [JsonPropertyName("maXa")]
    public int? MaXa { get; set; }
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp huyện dạng số lưu trong cơ sở dữ liệu.
    /// Xã, phường, thị trấn.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("MaHuyen",TypeName = "integer")]
    [JsonPropertyName("maHuyen")]
    public int? MaHuyen { get; set; }
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp tỉnh dạng số lưu trong cơ sở dữ liệu.
    /// Xã, phường, thị trấn.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("MaTinh",TypeName = "integer")]
    [JsonPropertyName("maTinh")]
    public int MaTinh { get; set; } 
    
    /// <summary>
    /// Là cấp của đơn vị hành chính.
    /// 1: Tỉnh, 2: Huyện, 3: Xã
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("Cap",TypeName = "integer")]
    [JsonPropertyName("cap")]
    public int Cap { get; set; }
    
    /// <summary>
    /// Là loại hình của đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [Column("LoaiHinh",TypeName = "varchar(50)")]
    [JsonPropertyName("loaiHinh")]
    [MaxLength(50)]
    public string LoaiHinh { get; set; } = string.Empty;
    
    /// <summary>
    /// Ngày hiệu lục của đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là ngày mà tên dơn vị hành chính được áp dụng
    /// theo các Nghị quyết của Quốc hội về việc sắp xếp, thay đổi các đơn vị hành chính.
    /// </remarks>
    [Column("NgayHieuLuc", TypeName = "timestamp with time zone")]
    [JsonPropertyName("ngayHieuLuc")]
    public DateTimeOffset NgayHieuLuc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Phiên bản có hiệu lực
    /// Các phiên bản khác cùng mã đơn vị hành chính sẽ không có hiệu lực.
    /// </summary>
    /// <remarks>
    /// Là ngày mà tên dơn vị hành chính được áp dụng
    /// theo các Nghị quyết của Quốc hội về việc sắp xếp, thay đổi các đơn vị hành chính.
    /// </remarks>
    [Column("HieuLuc", TypeName = "boolean")]
    [JsonPropertyName("hieuLuc")]
    public bool HieuLuc { get; set; } = true;
    
}