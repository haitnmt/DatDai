using System.Text;
using System.Xml.Linq;
using haihv.DatDai.Data.DanhMuc.Model;

namespace haihv.DatDai.Services.SyncDhvc;

public class CapHuyenRepository(string? url)
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _url = url ?? "https://danhmuchanhchinh.gso.gov.vn/DMDVHC.asmx";

    private const string SoapRequest = """
                                       <?xml version="1.0" encoding="utf-8"?>
                                       <soap12:Envelope 
                                       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                       xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                       xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                                         <soap12:Body>
                                           <DanhMucQuanHuyen xmlns="http://tempuri.org/">
                                               <DenNgay>string</DenNgay>
                                           </DanhMucQuanHuyen>
                                         </soap12:Body>
                                       </soap12:Envelope>
                                       """;

    private static List<DvhcDto> ParseProvinceResponse(string responseXml)
    {
        var capHuyens = new List<DvhcDto>();

        try
        {
            var doc = XDocument.Parse(responseXml);

            var tables = doc.Descendants("TABLE");

            capHuyens.AddRange(tables.Select(table => new DvhcDto()
            {
                MaKyHieu = table.Element("MaQuanHuyen")?.Value ?? string.Empty,
                MaTinh = table.Element("MaTinh")?.Value,
                MaHuyen = table.Element("MaQuanHuyen")?.Value,
                TenGiaTri = table.Element("TenQuanHuyen")?.Value ?? string.Empty,
                Cap = 2,
                LoaiHinh = table.Element("LoaiHinh")?.Value ?? string.Empty
            }));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi trong quá trình giải mã đơn vị hành chính từ XML: {ex.Message}", ex);
        }

        return capHuyens;
    }

    private async Task<List<DvhcDto>> GetAsync()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/DanhMucQuanHuyen");
            var content = new StringContent(SoapRequest, Encoding.UTF8, "application/soap+xml");

            var response = await _httpClient.PostAsync(_url, content);
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

//    public async Task CreateOrUpdateAsync()
//        => await new DvhcRepository(dataContextDiaChinh).CreateOrUpdateAsync(await GetAsync());
}