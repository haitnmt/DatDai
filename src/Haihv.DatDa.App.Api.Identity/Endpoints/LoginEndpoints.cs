using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Text.Json;
using System.Text.Json.Serialization;
using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Services;
using Haihv.DatDai.Lib.Model.Request.Identity;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;
namespace Haihv.DatDa.App.Api.Identity.Endpoints;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        
        app.MapPost("/login", Login);
    }
    
    private static Task<IResult> Login([FromBody] LoginRequest request,
        ILogger logger,
        IAuthenticateService authenticateService,
        TokenProvider tokenProvider,
        HttpContext httpContext)
    {
        var sw = Stopwatch.StartNew();
        // Xác thực trong cơ sở dữ liệu trước:
        var result = authenticateService.Authenticate(request).Result;
        return Task.FromResult(result.Match(
            s =>
            {
                var token = tokenProvider.GenerateToken(s);
                var userName = s.UserName;
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                if (elapsed > 1000)
                {
                    logger.Warning("Đăng nhập thành công: {Info} [{Elapsed} ms]",
                        httpContext.GetLogInfo(userName), elapsed);
                }
                else
                {
                    logger.Information("Đăng nhập thành công: {Info} [{Elapsed} ms]",
                        httpContext.GetLogInfo(userName), elapsed);
                }
                return Results.Ok(token);
            },
            ex =>
            {
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                logger.Error(ex, "Đăng nhập thất bại: {Info} [{Elapsed} ms]",
                    httpContext.GetLogInfo(request.Username),
                    elapsed);
                return Results.BadRequest(GetExceptionMessage(ex));
            }
        ));

        string GetExceptionMessage(Exception ex)
        {
            return ex switch
            {
                LdapException ldapException =>
                    ldapException.ErrorCode switch
                    {
                        49 => "Tên đăng nhập hoặc mật khẩu không chính xác!",
                        _ => ex.Message
                    },
                _ => ex.Message
            };
        }
    }
    
    
    private class LogInfo   
    {
        [JsonPropertyName("clientIp")]
        public string ClientIp { get; set; } = string.Empty;
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; } = string.Empty;
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        [JsonPropertyName("hashBody")]
        public string? HashBody { get; set; } = string.Empty;
        [JsonPropertyName("queryString")]
        public string? QueryString { get; set; } = string.Empty;
    }
    private static string GetLogInfo(this HttpContext httpContext, string? username = null)
    {
        return JsonSerializer.Serialize(new LogInfo
        {
            ClientIp = httpContext.Connection.LocalIpAddress?.ToString() ?? string.Empty,
            Username = username ?? httpContext.User.Identity?.Name ?? string.Empty,
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            Url = httpContext.Request.Path.Value ?? string.Empty,
            HashBody = httpContext.Request.Body.ToString().ComputeHash() ?? string.Empty,
            QueryString = httpContext.Request.QueryString.Value ?? string.Empty
        });
    }
    
}