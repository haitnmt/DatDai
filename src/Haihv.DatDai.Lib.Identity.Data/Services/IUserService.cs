using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public interface IUserService
{
    Task<Result<User>> CreateOrUpdateAsync(UserLdap userLdap, string? password = null);
}