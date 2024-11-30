using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Extension.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

/// <summary>
/// Dịch vụ để lấy và cập nhật thông tin quốc tịch từ REST Countries API.
/// </summary>
public class RestCountriesService(PostgreSqlConnection postgreSqlConnection, ElasticsearchClientSettings elasticsearchClientSettings)
{
    private readonly QuocTichDbContext _quocTichDbContext = new(postgreSqlConnection.PrimaryConnectionString, elasticsearchClientSettings);
    private readonly QuocTichDbContext _quocTichDbContextReadOnly = new(postgreSqlConnection.ReplicaConnectionString, elasticsearchClientSettings);
    private readonly HttpClient _httpClient = new();
    private const string Url = "https://restcountries.com/v3.1/independent?status=true&fields=ccn3,cca3,name";
    
    /// <summary>
    /// Lấy danh sách các quốc gia từ REST Countries API.
    /// </summary>
    /// <returns>Danh sách các quốc gia.</returns>
    private async Task<List<RestCountriesModel>> GetAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<RestCountriesModel>>(result);
            return data ?? [];
        }
        catch (Exception ex)
        {
            var message = $"Lỗi trong quá trình lấy dữ liệu từ REST Countries API: {ex.Message}";
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + ": " + message);
            return [];
            //throw new Exception($"Lỗi trong quá trình lấy dữ liệu từ REST Countries API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cập nhật thông tin quốc tịch vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="bulkSize">Kích thước của mỗi lô cập nhật.</param>
    /// <returns>Số lượng bản ghi đã chèn, cập nhật và bỏ qua.</returns>
    internal async Task<(int Insert, int Update, int Skip)> UpdateAsync(int bulkSize = 20)
    {
        var index = 0;
        var insert = 0;
        var update = 0;
        var skip = 0;
        var infoCountries = await GetAsync();
        // Cập nhật từng lô dữ liệu
        while (index < infoCountries.Count)
        {
            // Lấy lô dữ liệu cần cập nhật
            var bulkDvhcs = infoCountries.Skip(index).Take(bulkSize).ToList();
            
            // Cập nhật từng bản ghi
            foreach (var info in bulkDvhcs)
            {
                // Lấy tên quốc gia theo ngôn ngữ tiếng Anh
                const string keyEng = "eng";
                // Lấy tên quốc gia theo ngôn ngữ tiếng Anh
                var tenQuocTe = info.Name?.NativeName?.ContainsKey(keyEng) == true ? info.Name.NativeName[keyEng].Common : info.Name?.Common;
                var tenQuocTeDayDu = info.Name?.NativeName?.ContainsKey(keyEng) == true ? info.Name.NativeName[keyEng].Official : info.Name?.Official;
                // Kiểm tra xem quốc gia đã tồn tại trong cơ sở dữ liệu chưa
                _ = int.TryParse(info.Ccn3 ?? "0", out var ccn3);
                var existings = await _quocTichDbContextReadOnly.QuocTich
                    .Where(x =>
                    x.Ccn3 == ccn3 ||
                    x.Cca3 == info.Cca3)
                    .ToListAsync();
                // Nếu quốc gia đã tồn tại thì cập nhật thông tin
                var isExist = false;
                if (existings.Count > 0)
                {
                    foreach (var existing in existings)
                    {
                        if (existing.TenQuocTe == tenQuocTe &&
                            existing.TenQuocTeDayDu == tenQuocTeDayDu &&
                            existing.Ccn3 == ccn3 &&
                            existing.Cca3 == info.Cca3)
                        {
                            skip++;
                            isExist = true;
                            continue;
                        }
                        var newQuocTich = info.ToQuocTich();
                        existing.TenQuocGia = newQuocTich.TenQuocGia;
                        existing.TenDayDu = newQuocTich.TenDayDu;
                        existing.TenQuocTe = newQuocTich.TenQuocTe;
                        existing.TenQuocTeDayDu = newQuocTich.TenQuocTeDayDu;
                        existing.UpdatedAt = DateTimeOffset.UtcNow;
                        update++;
                    }
                    if (isExist) continue;
                }
                // Nếu quốc gia chưa tồn tại thì thêm mới
                _quocTichDbContext.QuocTich.Add(info.ToQuocTich());
                insert++;
            }
            
            // Lưu thay đổi vào cơ sở dữ liệu
            if (insert + update > 0)
                await _quocTichDbContext.SaveChangesAsync();
            index += bulkSize;
        }
        return (insert, update, skip);
    }
}