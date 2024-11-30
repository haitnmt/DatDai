using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Model.Request.Identity;

/// <summary>
/// Yêu cầu đăng nhập.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Tên người dùng.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu.
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } =  string.Empty;

    /// <summary>
    /// Ghi nhớ đăng nhập.
    /// </summary>
    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }
}