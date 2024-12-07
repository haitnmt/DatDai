using Haihv.DatDai.Lib.Identity.Data.Entities;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Data.Interfaces;

public interface IRefreshTokensService
{
    Result<RefreshToken> CreateToken(Guid userId);
    Result<RefreshToken> VerifyToken(Guid tokenId, string token);

    /// <summary>
    /// Xóa các token đã hết hạn.
    /// </summary>
    Task<int> DeleteExpiredTokens();
}