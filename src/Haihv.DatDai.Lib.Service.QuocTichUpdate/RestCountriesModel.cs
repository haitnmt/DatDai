using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.DanhMuc.Entities;
using Haihv.DatDai.Lib.Service.GoogleTranslate;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

/// <summary>
/// Model chứa thông tin quốc gia từ REST Countries API
/// </summary>
public class RestCountriesModel
{
    /// <summary>
    /// Thông tin tên quốc gia
    /// </summary>
    [JsonPropertyName("name")]
    public Name? Name { get; set; }

    /// <summary>
    /// Mã số quốc gia theo chuẩn UN M49
    /// </summary>
    [JsonPropertyName("ccn3")]
    public string? Ccn3 { get; set; }

    /// <summary>
    /// Mã quốc gia 3 ký tự theo chuẩn ISO 3166-1 alpha-3
    /// </summary>
    [JsonPropertyName("cca3")]
    public string? Cca3 { get; set; }
}

/// <summary>
/// Thông tin chi tiết về tên của quốc gia
/// </summary>
public class Name
{
    /// <summary>
    /// Tên thông dụng của quốc gia
    /// </summary>
    [JsonPropertyName("common")]
    public string? Common { get; set; }

    /// <summary>
    /// Tên chính thức của quốc gia
    /// </summary>
    [JsonPropertyName("official")]
    public string? Official { get; set; }

    /// <summary>
    /// Tên bản địa của quốc gia theo các ngôn ngữ khác nhau
    /// </summary>
    [JsonPropertyName("nativeName")]
    public Dictionary<string, LanguageInfo>? NativeName { get; set; }
}

/// <summary>
/// Thông tin tên quốc gia theo một ngôn ngữ cụ thể
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// Tên chính thức theo ngôn ngữ
    /// </summary>
    [JsonPropertyName("official")]
    public string? Official { get; set; }

    /// <summary>
    /// Tên thông dụng theo ngôn ngữ
    /// </summary>
    [JsonPropertyName("common")]
    public string? Common { get; set; }
}

public static class RestCountriesExtensions
{
    public static QuocTich ToQuocTich(this RestCountriesModel countryInfo)
    {
        const string keyVie = "vie";
        const string keyEng = "eng";
        
        // Lấy tên quốc gia theo ngôn ngữ tiếng Anh
        var tenQuocGiaQt = countryInfo.Name?.NativeName?.ContainsKey(keyEng) == true ? countryInfo.Name.NativeName[keyEng].Common : countryInfo.Name?.Common;
        var tenDayDuQt = countryInfo.Name?.NativeName?.ContainsKey(keyEng) == true ? countryInfo.Name.NativeName[keyEng].Official : countryInfo.Name?.Official;
    
        // Kiểm tra ngôn ngữ xem có tiêng Việt không
        var tenQuocGia = countryInfo.Name?.NativeName?.ContainsKey(keyVie) == true ? countryInfo.Name.NativeName[keyVie].Common : tenQuocGiaQt.Translate();
        var tenDayDu = countryInfo.Name?.NativeName?.ContainsKey(keyVie) == true ? countryInfo.Name.NativeName[keyVie].Official : tenDayDuQt.Translate();

        return new QuocTich
        {
            Ccn3 = int.Parse(countryInfo.Ccn3 ?? "0"),
            Cca3 = countryInfo.Cca3 ?? string.Empty,
            TenQuocGia = tenQuocGia,
            TenDayDu = tenDayDu,
            TenQuocTe = tenQuocGiaQt ?? string.Empty,
            TenQuocTeDayDu = tenDayDuQt ?? string.Empty
        };
    }
}