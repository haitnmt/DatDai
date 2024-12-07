using System.Text;
using System.Xml.Linq;
using Haihv.DatDai.Lib.Data.DanhMuc.Entities;

namespace Haihv.DatDai.Lib.Service.DvhcUpdate;

internal static class CapTinhService
{
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
                MaTinh = int.Parse(table.Element("MaTinh")?.Value?? "0"),
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

    public static async Task<List<Dvhc>> GetAsync()
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/DanhMucTinh");
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