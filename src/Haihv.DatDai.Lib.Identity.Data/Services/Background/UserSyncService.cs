using System.Diagnostics;
using Haihv.DatDai.Lib.Data.Base.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services.Background;

/// <summary>
/// Dịch vụ đồng bộ thông tin người dùng từ LDAP vào cơ sở dữ liệu.
/// Chỉ thực hiện các người dùng đã có trong dữ liệu.
/// </summary>
/// <param name="logger">Đối tượng ghi log.</param>
/// <param name="ldapContext"> Đối tượng kết nối LDAP.</param>
/// <param name="userService">Dịch vụ người dùng.</param>
public class UserSyncService(ILogger logger, ILdapContext ldapContext, IUserService userService)
    : BackgroundService
{
    private readonly int
        _defaultSecondDelay = ldapContext.LdapConnectionInfo.DefaultSyncDelay; // Thời gian đồng bộ mặc định

    private readonly UserLdapService _userLdapService = new(ldapContext);

    /// <summary>
    /// Thực thi đồng bộ người dùng từ LDAP vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="stoppingToken">Token hủy bỏ.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var count = await UpdateFromLdapAsync();
                sw.Stop();
                logger.Debug(
                    "Đồng bộ thông tin người dùng từ LDAP vào cơ sở dữ liệu thành công [{count} bản ghi] [{Elapsed} ms]",
                    count, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.Error(ex,
                    "Lỗi khi đồng bộ thông tin người dùng từ LDAP vào cơ sở dữ liệu {LdapInfo} [{Elapsed} ms]",
                    ldapContext.ToLogInfo(), sw.ElapsedMilliseconds);
            }

            var delayTime = SettingExtensions.GetDelayTime(days: 0, seconds: _defaultSecondDelay);
            logger.Debug(
                "Đồng bộ thông tin người dùng từ LDAP vào cơ sở dữ liệu lần tiếp theo vào lúc: {NextTime:dd:MM:yyyy HH:mm:ss zz}",
                delayTime.NextSyncTime);
            await Task.Delay(delayTime.Delay, stoppingToken);
        }
    }

    /// <summary>
    /// Cập nhật người dùng từ LDAP.
    /// </summary>
    /// <returns>
    /// Số lượng người dùng đã cập nhật.
    /// </returns>
    private async Task<int> UpdateFromLdapAsync()
    {
        var result = 0;
        // Lấy danh sách người dùng từ dữ liệu:
        var users = await userService.GetAsync();
        // Duyệt qua từng người dùng
        foreach (var user in users)
        {
            try
            {
                // Lấy thông tin người dùng từ LDAP
                var userLdap = string.IsNullOrWhiteSpace(user.DistinguishedName)
                    ? await _userLdapService
                        .GetByPrincipalNameAsync(user.UserName, user.UpdatedAt.UtcDateTime)
                    : await _userLdapService
                        .GetByPrincipalNameAsync(user.DistinguishedName, user.UpdatedAt.UtcDateTime);
                // Cập nhật thông tin người dùng
                if (userLdap is null) continue;
                await userService.CreateOrUpdateAsync(userLdap);
                result++;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Lỗi khi cập nhật thông tin người dùng {UserName}", user.UserName);
                throw;
            }
        }

        return result;
    }
}