using System.Text.Json;
using System.Web;

namespace Haihv.DatDai.Lib.Service.GoogleTranslate;

public static class TranslateExtensions
{
    public static string Translate(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }
        var url =
            $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=vi&dt=t&q={HttpUtility.UrlEncode(text)}";
        using var client = new HttpClient();
        var response = client.GetAsync(url).Result;
        var result = response.Content.ReadAsStringAsync().Result;
        var data = JsonSerializer.Deserialize<List<object>>(result);
        if (data == null || data.Count == 0)
        {
            return text;
        }
        var translatedText = ((JsonElement)data[0])[0][0].GetString();
        return translatedText ?? text;
    }
}