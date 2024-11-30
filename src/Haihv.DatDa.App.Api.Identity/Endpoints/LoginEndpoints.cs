using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Text.Json;
using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.Login.Ldap;
using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Ldap;
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
    
    private static IResult Login([FromBody]LoginRequest request, ILogger logger, ILdapContext ldapContext, TokenProvider tokenProvider, HttpContext httpContext)
    {
        if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            logger.Warning("Tài khoản hoặc mật khẩu trống: {Info}", httpContext.GetLogInfo(request.Username));
            return Results.BadRequest("Tài khoản hoặc mật khẩu trống");
        }
        var sw = Stopwatch.StartNew();
        var result = request.LoginByLdap(ldapContext);
        return result.Match(
            s =>
            {
                var userPrincipalName = s.UserPrincipalName!;
                var token = tokenProvider.GenerateToken(userPrincipalName);
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                if (elapsed > 1000)
                {
                    logger.Warning("Đăng nhập thành công: {Info} [{Elapsed} ms]", httpContext.GetLogInfo(userPrincipalName), elapsed);
                }
                else
                {
                    logger.Information("Đăng nhập thành công: {Info}", httpContext.GetLogInfo(userPrincipalName));
                }
                return Results.Ok(token);
            },
            ex =>
            {
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                if (ex is LdapException ldapException)
                {
                    
                    if (ldapException.ErrorCode == 49)
                    {
                        logger.Warning(ldapException, "Đăng nhập thất bại: {Info}",
                            httpContext.GetLogInfo(request.Username));
                        return Results.BadRequest($"[Error-code: {ldapException.ErrorCode}] {ldapException.Message} Tài khoản hoặc mật khẩu không đúng");
                    }
                    logger.Error(ldapException, "Lỗi khi kết nối đến LDAP: {LdapInfo} [{Elapsed} ms]", 
                        httpContext.GetLogInfo(request.Username),
                        elapsed);
                    return Results.BadRequest($"[Error-code: {ldapException.ErrorCode}] {ldapException.Message}");
                }
                logger.Error(ex,"Đăng nhập thất bại: {LdapInfo} [{Elapsed} ms]",
                    httpContext.GetLogInfo(request.Username),
                    elapsed);
                return Results.BadRequest(ex.Message);
            }
        );
    }
    private class LogInfo   
    {
        public string ClientIp { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public string HashBody { get; set; } = string.Empty;
        
        public string QueryString { get; set; } = string.Empty;
    }
    private static string GetLogInfo(this HttpContext httpContext, string username)
    {
        return JsonSerializer.Serialize(new LogInfo
        {
            ClientIp = httpContext.Connection.LocalIpAddress?.ToString() ?? string.Empty,
            Username = username,
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            HashBody = httpContext.Request.Body.ToString().ComputeHash() ?? string.Empty,
            QueryString = httpContext.Request.QueryString.Value ?? string.Empty
        });
    }
}