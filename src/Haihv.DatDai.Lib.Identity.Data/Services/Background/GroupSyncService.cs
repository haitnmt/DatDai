using System.Diagnostics;
using Haihv.DatDai.Lib.Data.Base.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services.Background;

/// <summary>
/// Dịch vụ đồng bộ nhóm từ LDAP vào cơ sở dữ liệu.
/// </summary>
/// <param name="logger">Đối tượng ghi log.</param>
/// <param name="ldapContext"> Đối tượng kết nối LDAP.</param>
/// <param name="groupService">Dịch vụ nhóm.</param>
public class GroupSyncService(ILogger logger, ILdapContext ldapContext, IGroupService groupService)
    : BackgroundService
{
    private readonly int _defaultSecondDelay = ldapContext.LdapConnectionInfo.DefaultSyncDelay; // Thời gian đồng bộ mặc định
    private readonly GroupLdapService _groupLdapService = new(ldapContext);

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
                var count = await UpdateFromLdapAsync();
                sw.Stop();
                logger.Debug(
                    "Đồng bộ nhóm từ LDAP vào cơ sở dữ liệu thành công [{count} bản ghi] [{Elapsed} ms]",
                    count, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.Error(ex, "Lỗi khi đồng bộ nhóm từ LDAP vào cơ sở dữ liệu {LdapInfo} [{Elapsed} ms]",
                    ldapContext.ToLogInfo(), sw.ElapsedMilliseconds);
            }
            var delayTime = SettingExtensions.GetDelayTime(days: 0, seconds: _defaultSecondDelay);
            logger.Debug(
                "Đồng bộ các nhóm từ LDAP vào cơ sở dữ liệu lần tiếp theo vào lúc: {NextTime:dd:MM:yyyy HH:mm:ss zz}", delayTime.NextSyncTime);
            await Task.Delay(delayTime.Delay, stoppingToken);
        }
    }

    /// <summary>
    /// Cập nhật nhóm từ LDAP.
    /// </summary>
    /// <returns>
    /// Số lượng nhóm đã cập nhật.
    /// </returns>
    private async Task<int> UpdateFromLdapAsync()
    {
        var result = 0;
        Queue<string> groupQueue = new(); // Hàng đợi để duyệt theo tầng
        // Lấy nhóm gốc
        groupQueue.Enqueue(_groupLdapService.RootGroupDn);
        var whenChanged = groupService.GetLastSyncTime(1)?.UtcDateTime ?? DateTime.MinValue;
        if (whenChanged != default && whenChanged != DateTime.MinValue)
        {
            whenChanged = whenChanged.AddSeconds(1);
        }

        List<string> processedGroupDns = [];
        
        while (groupQueue.Count > 0)
        {
            // Lấy nhóm hiện tại từ hàng đợi
            var currentGroupDn = groupQueue.Dequeue();
            processedGroupDns.Add(currentGroupDn);
            // Thêm nhóm vào danh sách kết quả nếu thoả mãn điều kiện
            var currentGroup = await _groupLdapService.GetByDnAsync(currentGroupDn, whenChanged);
            // Thêm vào danh sách kết quả nếu có nhóm thoả mãn 
            if (currentGroup is not null)
            {
                await groupService.CreateOrUpdateAsync(currentGroup);
                result++;
            }

            // Thêm các nhóm vừa tìm được vào hàng đợi để xử lý đệ quy, **bỏ qua bộ lọc whenChanged tại đây**
            // Lấy danh sách các DN của nhóm con
            var dns = await _groupLdapService.GetDnByMemberOfAsync(currentGroupDn);
            foreach (var dn in dns.Where(dn => !processedGroupDns.Contains(dn) && !groupQueue.Contains(dn)))
            {
                groupQueue.Enqueue(dn); // Thêm DN của nhóm con vào hàng đợi
            }
        }

        return result;
    }
}