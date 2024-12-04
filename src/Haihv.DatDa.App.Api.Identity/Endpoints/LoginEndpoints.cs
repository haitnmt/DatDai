using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Text.Json;
using System.Text.Json.Serialization;
using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Data.Services;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
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
        IAuthenticateLdapService authenticateLdapService,
        IAuthenticateService authenticateService,
        IUserService userService,
        TokenProvider tokenProvider,
        HttpContext httpContext)
    {
        if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            logger.Warning("Tài khoản hoặc mật khẩu trống: {Info}", httpContext.GetLogInfo(request.Username));
            return Task.FromResult(Results.BadRequest("Tài khoản hoặc mật khẩu trống"));
        }
        var sw = Stopwatch.StartNew();
        // Xác thực trong cơ sở dữ liệu trước:
        var resultDb = authenticateService.Authenticate(request.Username, request.Password).Result;
        return Task.FromResult(resultDb.Match(
            s =>
            {
                var userName = s.UserName!;
                var token = tokenProvider.GenerateToken(userName);
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                if (elapsed > 1000)
                {
                    logger.Warning("Đăng nhập thành công: {Info} [{Elapsed} ms]",
                        httpContext.GetLogInfo(userName), elapsed);
                }
                else
                {
                    logger.Information("Đăng nhập thành công: {Info}",
                        httpContext.GetLogInfo(userName));
                }

                return Results.Ok(token);
            },
            ex =>
            {
                var result = authenticateLdapService.Authenticate(request.Username, request.Password);
                return Task.FromResult(result.Match(
                    s =>
                    {
                        var userPrincipalName = s.UserPrincipalName!;
                        var token = tokenProvider.GenerateToken(userPrincipalName);
                        sw.Stop();
                        var elapsed = sw.ElapsedMilliseconds;
                        if (elapsed > 1000)
                        {
                            logger.Warning("Đăng nhập thành công: {Info} [{Elapsed} ms]",
                                httpContext.GetLogInfo(userPrincipalName), elapsed);
                        }
                        else
                        {
                            logger.Information("Đăng nhập thành công: {Info}",
                                httpContext.GetLogInfo(userPrincipalName));
                        }

                        _ = userService.CreateOrUpdateAsync(s, request.Password);
                        return Results.Ok(token);
                    },
                    exLdap =>
                    {
                        sw.Stop();
                        var elapsed = sw.ElapsedMilliseconds;
                        if (exLdap is LdapException ldapException)
                        {

                            if (ldapException.ErrorCode == 49)
                            {
                                logger.Warning(ldapException, "Đăng nhập thất bại: {Info}",
                                    httpContext.GetLogInfo(request.Username));
                                return Results.BadRequest(
                                    $"[Error-code: {ldapException.ErrorCode}] {ldapException.Message} Tài khoản hoặc mật khẩu không đúng");
                            }

                            logger.Error(ldapException, "Lỗi khi kết nối đến LDAP: {LdapInfo} [{Elapsed} ms]",
                                httpContext.GetLogInfo(request.Username),
                                elapsed);
                            return Results.BadRequest(
                                $"[Error-code: {ldapException.ErrorCode}] {ldapException.Message}");
                        }

                        logger.Error(exLdap, "Đăng nhập thất bại: {LdapInfo} [{Elapsed} ms]",
                            httpContext.GetLogInfo(request.Username),
                            elapsed);
                        return Results.BadRequest(exLdap.Message);
                    }
                ));
            }
        ));

    }
    
    private async Task<string> Authenticate(LoginRequest loginRequest)
    {
        var user = await userService.GetByUserNameAsync(loginRequest.Username);
        if (user == null)
        {
            return "Tài khoản không tồn tại";
        }

        if (user.Password != loginRequest.Password)
        {
            return "Mật khẩu không đúng";
        }

        return tokenProvider.GenerateToken(user.UserName!);
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