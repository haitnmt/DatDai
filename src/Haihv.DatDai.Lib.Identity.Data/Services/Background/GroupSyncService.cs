using System.Diagnostics;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services.Background;

/// <summary>
/// Dịch vụ đồng bộ nhóm từ LDAP vào cơ sở dữ liệu.
/// </summary>
/// <param name="logger">Đối tượng ghi log.</param>
/// <param name="groupLdapService">Dịch vụ LDAP nhóm.</param>
/// <param name="groupService">Dịch vụ nhóm.</param>
public class GroupSyncService(ILogger logger, IGroupLdapService groupLdapService, IGroupService groupService)
    : BackgroundService
{
    private const int DefaultSecondDelay = 300; // 3 phút

    /// <summary>
    /// Thực thi đồng bộ nhóm từ LDAP vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="stoppingToken">Token hủy bỏ.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var whenChanged = groupService.GetLastSyncTime(1)?.UtcDateTime ?? DateTime.MinValue;
                var groupLdaps = await groupLdapService.GetAllGroupsLdapByRecursiveAsync(whenChanged);
                await groupService.CreateOrUpdateAsync(groupLdaps);
                sw.Stop();
                logger.Information(
                    "Đồng bộ nhóm từ LDAP vào cơ sở dữ liệu thành công [{Elapsed} ms], sẽ đồng bộ lại sau {SecondDelay} giây",
                    sw.ElapsedMilliseconds,
                    DefaultSecondDelay);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Lỗi khi đồng bộ nhóm từ LDAP vào cơ sở dữ liệu");
            }

            await Task.Delay(TimeSpan.FromSeconds(DefaultSecondDelay), stoppingToken);
        }
    }
}