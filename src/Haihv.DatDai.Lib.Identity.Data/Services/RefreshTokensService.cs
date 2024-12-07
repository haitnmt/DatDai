using System.Security.Cryptography;
using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entities;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using LanguageExt.Common;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public sealed class RefreshTokensService(ILogger logger,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider,
    int tokenExpirationInDays = 7) : IRefreshTokensService
{
    
    private readonly IdentityDbContext _dbContextRead =
        new(postgreSqlConnection.ReplicaConnectionString, auditDataProvider);

    private readonly IdentityDbContext _dbContextWrite =
        new(postgreSqlConnection.PrimaryConnectionString, auditDataProvider);
    
    private readonly TimeSpan _tokenExpiration = tokenExpirationInDays == default
        ? TimeSpan.FromDays(7)
        : TimeSpan.FromDays(tokenExpirationInDays);
    
    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    
    public Result<RefreshToken> CreateToken(Guid userId)
    {
        try
        {
            // Tạo token mới cho user (sử dụng sinh chuỗi ngẫu nhiên)
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateToken(),
                Expires = DateTimeOffset.Now.Add(_tokenExpiration)
            };
            _dbContextWrite.RefreshTokens.Add(refreshToken);
            _dbContextWrite.SaveChangesAsync();
            return refreshToken;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Create token failed [{userId}]", userId);
            return new Result<RefreshToken>(ex);
        }
    }
    public Result<RefreshToken> VerifyToken(Guid tokenId, string token)
    {
        try
        {
            var refreshToken = GetAndDeleteToken(tokenId, token);
            // Nếu token hợp lệ thì tạo token mới
            if (refreshToken != null) return CreateToken(refreshToken.UserId);
            // Nếu token không hợp lệ thì trả về lỗi
            logger.Warning("Token not found or expires [{token}]", token);
            return new Result<RefreshToken>(new Exception("Token not found"));
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Verify token failed [{token}]", token);
            return new Result<RefreshToken>(ex);
        }
    }
    
    private RefreshToken? GetAndDeleteToken(Guid tokenId, string token)
    {
        var refreshToken = _dbContextRead.RefreshTokens
            .FirstOrDefault(rt => rt.Id == tokenId && rt.Token == token);
        if (refreshToken is null)
        {
            return null;
        }
        _dbContextWrite.RefreshTokens.Remove(refreshToken);
        _ = _dbContextWrite.SaveChangesAsync();
        return refreshToken.Expires < DateTime.UtcNow ? null : refreshToken;
    }
    
    /// <summary>
    /// Xóa các token đã hết hạn.
    /// </summary>
    public async Task<int> DeleteExpiredTokens()
    {
        var expiredTokens = _dbContextRead.RefreshTokens
            .Where(rt => rt.Expires < DateTime.UtcNow);
        _dbContextWrite.RefreshTokens.RemoveRange(expiredTokens);
        await _dbContextWrite.SaveChangesAsync();
        return expiredTokens.Count();
    }
}