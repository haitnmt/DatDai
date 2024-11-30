using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Haihv.DatDai.Lib.Model.Request.Identity;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Extension.Login.Ldap;

/// <summary>
/// Lớp chứa các phương thức đăng nhập bằng LDAP.
/// </summary>
public static class Login
{
    /// <summary>
    /// Đăng nhập bằng LDAP với yêu cầu đăng nhập.
    /// </summary>
    /// <param name="request">Yêu cầu đăng nhập chứa tên người dùng và mật khẩu.</param>
    /// <param name="ldapContext">Ngữ cảnh LDAP để xác thực.</param>
    /// <returns>Trả về true nếu xác thực thành công, ngược lại trả về false.</returns>
    public static Result<UserLdap> LoginByLdap(this LoginRequest request, ILdapContext ldapContext)
    {
        return ldapContext.Authenticate(request.Username, request.Password);
    }
    
    /// <summary>
    /// Đăng nhập bằng LDAP với ngữ cảnh LDAP.
    /// </summary>
    /// <param name="ldapContext">Ngữ cảnh LDAP để xác thực.</param>
    /// <param name="request">Yêu cầu đăng nhập chứa tên người dùng và mật khẩu.</param>
    /// <returns>Trả về true nếu xác thực thành công, ngược lại trả về false.</returns>
    public static Result<UserLdap> LoginByLdap(this ILdapContext ldapContext, LoginRequest request)
    {
        return ldapContext.Authenticate(request.Username, request.Password);
    }
}