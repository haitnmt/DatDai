using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Haihv.DatDai.Lib.Extension.String;

public static class ComputeHashExtensions
{
    private static string ComputeHash(this string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }
    public static string? ComputeHash<T>(this T? input)
    {
        if (input == null) return null;
        if (input is string stringInput)
        {
            return ComputeHash(stringInput);
        }
        var jsonString = JsonSerializer.Serialize(input);
        var inputBytes = Encoding.UTF8.GetBytes(jsonString);
        var hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }
    public static string? ComputeHash<T>(string? txt = null, T? obj = default)
    {
        var jsonString = JsonSerializer.Serialize(obj) + txt;
        if (string.IsNullOrWhiteSpace(jsonString)) return null;

        var inputBytes = Encoding.UTF8.GetBytes(jsonString);
        var hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }
}