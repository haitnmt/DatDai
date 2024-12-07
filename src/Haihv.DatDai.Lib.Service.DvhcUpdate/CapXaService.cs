using System.Text;
using System.Xml.Linq;
using Haihv.DatDai.Lib.Data.DanhMuc.Entities;

namespace Haihv.DatDai.Lib.Service.DvhcUpdate;

internal static class CapXaService
{
    private const string Url = "https://danhmuchanhchinh.gso.gov.vn/DMDVHC.asmx";

    private const string SoapRequest = """
                                       <?xml version="1.0" encoding="utf-8"?>
                                       <soap12:Envelope 
                                       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                       xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                       xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                                         <soap12:Body>
                                           <DanhMucPhuongXa xmlns="http://tempuri.org/">
                                             <DenNgay>string</DenNgay>
                                           </DanhMucPhuongXa>
                                         </soap12:Body>
                                       </soap12:Envelope>
                                       """;
    private static List<Dvhc> ParseProvinceResponse(string responseXml)
    {
        var capXas = new List<Dvhc>();
            
        try
        {
            var doc = XDocument.Parse(responseXml);

            var tables = doc.Descendants("TABLE");

            capXas.AddRange(tables.Select(table => new Dvhc()
            {
                MaTinh = int.Parse(table.Element("MaTinh")?.Value?? "0"),
                MaHuyen = table.Element("MaQuanHuyen")?.Value != null ? int.Parse(table.Element("MaQuanHuyen")?.Value!) : null,
                MaXa = table.Element("MaPhuongXa")?.Value != null ? int.Parse(table.Element("MaPhuongXa")?.Value!) : null,
                Cap = 3,
                TenGiaTri = table.Element("TenPhuongXa")?.Value ?? string.Empty,
                LoaiHinh = table.Element("LoaiHinh")?.Value ?? string.Empty
            }));
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi trong quá trình giải mã đơn vị hành chính từ XML: {ex.Message}", ex);
        }

        return capXas;
    }
    
    public static async Task<List<Dvhc>> GetAsync()
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/DanhMucPhuongXa");
                var content = new StringContent(SoapRequest, Encoding.UTF8, "application/soap+xml");

                var response = await httpClient.PostAsync(Url, content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return ParseProvinceResponse(responseString);
            }
            catch (Exception e)
            {
                retryCount++;
                Console.WriteLine($"Lỗi trong quá trình lấy thông đơn vị hành chính tin từ API: {e.Message}", e);
                var delay = TimeSpan.FromSeconds(30 * retryCount);
                delay = delay > TimeSpan.FromHours(12) ? TimeSpan.FromHours(12) : delay;
                Console.WriteLine("Thử lại sau {0} giây...", delay);
                await Task.Delay(delay);
            }
        }
    }
}