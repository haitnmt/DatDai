using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp đại diện cho thông tin về đơn vị hành chính trong cơ sở dữ liệu đất đai.
/// </summary>
public class Dvhc : BaseDto, IDanhMuc
{
    /// <summary>
    /// Là tên đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("tenDvhc")]
    [MaxLength(255)]
    public string TenGiaTri { get; set; } = string.Empty;
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp xã:
    /// Xã, phường, thị trấn.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maXa")]
    [MaxLength(5)]
    public string? MaXa { get; set; }

    /// <summary>
    /// Là mã đơn vị hành chính cấp huyện:
    /// Quận, huyện, thị xã, thành phố trực thuộc tỉnh, thành phố trực thuộc thành phố thuộc trung ương.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maHuyen")]
    [MaxLength(3)]
    public string? MaHuyen { get; set; }
        
    /// <summary>
    /// Là mã đơn vị hành chính cấp tỉnh:
    /// Tỉnh, thành phố trực thuộc trung ương.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maTinh")]
    [MaxLength(2)]
    public string? MaTinh { get; set; }
    
    /// <summary>
    /// Là cấp của đơn vị hành chính.
    /// 1: Tỉnh, 2: Huyện, 3: Xã
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("cap")]
    public int Cap { get; set; }
    
    /// <summary>
    /// Là loại hình của đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
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
    [JsonPropertyName("hieuLuc")]
    public bool HieuLuc { get; set; } = true;

    [JsonPropertyName("maDvhc")] public string MaKyHieu => this.GetMaDvhc();
}

public static class DvhcExtensions
{
    public static string GetMaDvhc(this Dvhc dvhc)
    {
        return dvhc switch
        {
            { MaXa: not null } => dvhc.MaXa,
            { MaHuyen: not null } => dvhc.MaHuyen,
            _ => dvhc.MaTinh ?? string.Empty
        };
    }
}