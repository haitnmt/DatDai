using System.Text.Json.Serialization;

namespace haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp đại diện cho thông tin về đơn vị hành chính trong cơ sở dữ liệu đất đai.
/// </summary>
public class DvhcDto : DanhMucDto
{
    /// <summary>
    /// Là mã đơn vị hành chính
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maDvhc")]
    public override string MaKyHieu { get; set; } = string.Empty;
    
    /// <summary>
    /// Là tên đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("tenDvhc")]
    public override string TenGiaTri { get; set; } = string.Empty;
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp xã:
    /// Xã, phường, thị trấn.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maXa")]
    public string? MaXa { get; init; }
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp huyện:
    /// Quận, huyện, thị xã, thành phố trực thuộc tỉnh, thành phố trực thuộc thành phố thuộc trung ương.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maHuyen")]
    public string? MaHuyen { get; init; } 
    
    /// <summary>
    /// Là mã đơn vị hành chính cấp tỉnh:
    /// Tỉnh, thành phố trực thuộc trung ương.
    /// </summary>
    /// <remarks>
    /// Là mã số đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("maTinh")]
    public string? MaTinh { get; init; }
    
    /// <summary>
    /// Là cấp của đơn vị hành chính.
    /// 1: Tỉnh, 2: Huyện, 3: Xã
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("cap")]
    public int Cap { get; init; } = 0;
    
    /// <summary>
    /// Là loại hình của đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là tên đơn vị hành chính theo quy định của Thủ tướng Chính phủ
    /// về việc ban hành bảng danh mục và mã số các đơn vị hành chính Việt Nam.
    /// </remarks>
    [JsonPropertyName("loaiHinh")]
    public string LoaiHinh { get; init; } = string.Empty;
    
    /// <summary>
    /// Ngày hiệu lục của đơn vị hành chính.
    /// </summary>
    /// <remarks>
    /// Là ngày mà tên dơn vị hành chính được áp dụng
    /// theo các Nghị quyết của Quốc hội về việc sắp xếp, thay đổi các đơn vị hành chính.
    /// </remarks>
    [JsonPropertyName("ngayHieuLuc")]
    public DateTime NgayHieuLuc { get; set; } = DateTime.Today;

    /// <summary>
    /// Phiên bản có hiệu lực
    /// Các phiên bản khác cùng mã đơn vị hành chính sẽ không có hiệu lực.
    /// </summary>
    /// <remarks>
    /// Là ngày mà tên dơn vị hành chính được áp dụng
    /// theo các Nghị quyết của Quốc hội về việc sắp xếp, thay đổi các đơn vị hành chính.
    /// </remarks>
    [JsonPropertyName("hieuLuc")]
    public Boolean HieuLuc { get; set; } = true;
}