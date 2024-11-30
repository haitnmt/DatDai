using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.Login.Ldap;
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
            logger.Warning("Tài khoản hoặc mật khẩu trống");
            return Results.BadRequest("Tài khoản hoặc mật khẩu trống");
        }
        var result = request.LoginByLdap(ldapContext);
        return result.Match(
        
            s =>
            {
                var userPrincipalName = s.UserPrincipalName!;
                var token = tokenProvider.GenerateToken(userPrincipalName);
                logger.Information("{username} đăng nhập thành công.", userPrincipalName);
                return Results.Ok(token);
            },
            ex =>
            {
                logger.Warning("{username} đăng nhập thất bại.", request.Username);
                return Results.BadRequest("Tài khoản hoặc mật khẩu không đúng");
            }
        );
    }
}