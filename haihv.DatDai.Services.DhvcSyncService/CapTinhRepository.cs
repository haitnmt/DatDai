using System.Text;
using System.Xml.Linq;
using haihv.DatDai.Data.DanhMuc.Model;
using haihv.DatDai.Data.DanhMuc.Services;

namespace haihv.DatDai.Services.SyncDhvc;

internal class CapTinhRepository(DanhMucDbContext dbContext)
{
    private readonly DvhcService _dvhcService = new(dbContext);
    private readonly HttpClient _httpClient = new();
    private const string Url = "https://danhmuchanhchinh.gso.gov.vn/DMDVHC.asmx";

    private const string SoapRequest = """
                                       <?xml version="1.0" encoding="utf-8"?>
                                           <soap12:Envelope 
                                             xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                             xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                             xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                                             <soap12:Body>
                                               <DanhMucTinh xmlns="http://tempuri.org/">
                                                 <DenNgay>string</DenNgay>
                                               </DanhMucTinh>
                                             </soap12:Body>
                                           </soap12:Envelope>
                                       """;
    
    private static List<Dvhc> ParseProvinceResponse(string responseXml)
    {
        var provinces = new List<Dvhc>();
        try
        {
            var doc = XDocument.Parse(responseXml);
            var tables = doc.Descendants("TABLE");
            provinces.AddRange(tables.Select(table => new Dvhc()
            {
                MaTinh = table.Element("MaTinh")?.Value,
                TenGiaTri = table.Element("TenTinh")?.Value ?? string.Empty,
                Cap = 1,
                LoaiHinh = table.Element("LoaiHinh")?.Value ?? string.Empty
            }));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi trong quá trình giải mã đơn vị hành chính từ XML: {ex.Message}", ex);
        }

        return provinces;
    }

    private async Task<List<Dvhc>> GetAsync()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/DanhMucTinh");
            var content = new StringContent(SoapRequest, Encoding.UTF8, "application/soap+xml");
            var response = await _httpClient.PostAsync(Url, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return ParseProvinceResponse(responseString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception($"Lỗi trong quá trình lấy thông đơn vị hành chính tin từ API: {e.Message}", e);
        }
    }

    public async Task CreateOrUpdateAsync() 
        => await _dvhcService.UpdateDvhcAsync(await GetAsync());
}