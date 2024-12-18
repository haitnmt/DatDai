using System.Diagnostics;
using System.Text;
using Audit.Core;
using Haihv.DatDai.Lib.Data.Base.Extensions;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

/// <summary>
/// Dịch vụ cập nhật dữ liệu quốc tịch.
/// </summary>
public class QuocTichUpdateService(
    ILogger logger,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider) : BackgroundService
{
    private const int DayDelay = 30;

    /// <summary>
    /// Thực thi dịch vụ cập nhật dữ liệu quốc tịch
    /// </summary>
    /// <param name="stoppingToken">Token để dừng tác vụ.</param>
    /// <returns>Task không đồng bộ.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Thiết lập hỗ trợ Console cho tiếng Việt
        Console.OutputEncoding = Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            var success = true;
            try
            {
                if (string.IsNullOrWhiteSpace(postgreSqlConnection.PrimaryConnectionString))
                {
                    logger.Error("Chuỗi kết nối PostgreSQL không hợp lệ");
                    success = false;
                }
                else
                {
                    var message = await SyncData();
                    sw.Stop();
                    message += $" [{sw.Elapsed.TotalSeconds}s]";
                    logger.Debug(message);
                }
            }
            catch (Exception ex)
            {
                success = false;
                sw.Stop();
                logger.Error(ex, $"Cập nhật dữ liệu quốc tịch thất bại: [{ex.Message}][{sw.Elapsed.TotalSeconds}s]");
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = SettingExtensions.GetDelayTime(days: DayDelay);
                logger.Debug(
                    $"Lần cập nhật dữ liệu quốc tịch tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                logger.Error(
                    $"Sẽ thực hiện lại cập nhật dữ liệu quốc tịch lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    /// <summary>
    /// Đồng bộ dữ liệu quốc tịch từ dịch vụ RestCountries.
    /// </summary>
    /// <returns>
    /// Một chuỗi thông báo kết quả của quá trình đồng bộ.
    /// </returns>
    private async Task<string> SyncData()
    {
        logger.Debug("Bắt đầu cập nhật dữ liệu quốc tịch");
        var service = new RestCountriesService(postgreSqlConnection, auditDataProvider);
        var (insert, update, skip) = await service.UpdateAsync();
        var message = $"Cập nhật dữ liệu quốc tịch thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]";
        return message;
    }
}