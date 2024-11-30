using System.Diagnostics;
using System.Text;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Data.DanhMuc.Services;
using Haihv.DatDai.Lib.Extension.Configuration;
using Haihv.DatDai.Lib.Service.DvhcUpdate.Entities;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haihv.DatDai.Lib.Service.DvhcUpdate;

/// <summary>
/// Dịch vụ cập nhật dữ liệu đơn vị hành chính.
/// </summary>
public class DvhcUpdateService(ILogger logger, PostgreSqlConnection postgreSqlConnection, ElasticsearchClientSettings elasticsearchClientSettings) : BackgroundService
{

    private const int DayDelay = 1;
    /// <summary>
    /// Thực thi dịch vụ cập nhật dữ liệu đơn vị hành chính.
    /// </summary>
    /// <param name="stoppingToken">Token để dừng tác vụ.</param>
    /// <returns>Tác vụ không đồng bộ.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Thiết lập hỗ trợ Console cho tiếng Việt
        Console.OutputEncoding = Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            bool success;
            logger.Information("Bắt đầu đồng bộ dữ liệu đơn vị hành chính");
            try
            {
                (var message, success) = await SyncData();
                sw.Stop();
                message += $" [{sw.Elapsed.TotalSeconds}s]";
                if (success)
                {
                    logger.Information(message);
                }
                else
                {
                    logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                success = false;
                logger.Error(ex,$"Đồng bộ dữ liệu đơn vị hành chính thất bại [{ex.Message}][{sw.Elapsed.TotalSeconds}s]");
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
                logger.Information(
                    $"Lần đồng bộ dữ liệu đơn vị hành chính tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");            }
            else
            {
                logger.Warning(
                    $"Sẽ thực hiện lại đồng bộ dữ liệu đơn vị hành chính lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    /// <summary>
    /// Đồng bộ dữ liệu đơn vị hành chính.
    /// </summary>
    /// <returns>
    /// Chuỗi thông báo kết quả đồng bộ.
    /// </returns>
    private async Task<(string message, bool sucsses)> SyncData()
    {
       
        List<Task<List<Dvhc>>> tasks =
        [
            CapTinhEntity.GetAsync(),
            CapHuyenEntity.GetAsync(),
            CapXaEntity.GetAsync()
        ];
        await Task.WhenAll(tasks);
        var dvhcs = tasks.SelectMany(x => x.Result).ToList();
        return await CreateOrUpdateAsync(dvhcs);
    }
    private async Task<(string message, bool sucsses)> CreateOrUpdateAsync(List<Dvhc> dvhcs)
    {
        string message;
        if (string.IsNullOrWhiteSpace(postgreSqlConnection.PrimaryConnectionString))
        {
            message = "Connection string is null or empty";
            logger.Error(message);
            return  (message, false);
        }
        var dvhcService = new DvhcService(postgreSqlConnection, elasticsearchClientSettings);
        var (insert, update, skip) = await dvhcService.UpdateDvhcAsync(dvhcs);
        message = $"Đồng bộ dữ liệu đơn vị hành chính thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]";
        return  (message, true);
    }
}