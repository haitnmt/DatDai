using System.Diagnostics;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Services;
using Haihv.DatDai.Lib.Extension.Configuration;
using ILogger = Serilog.ILogger;

namespace Haihv.DatDai.App.Background.Initiation.Entities;

/// <summary>
/// Dịch vụ cập nhật dữ liệu dân tộc.
/// </summary>
public class DanTocUpdateService(
    ILogger logger,
    PostgreSqlConnection postgreSqlConnection, 
    ElasticsearchClientSettings elasticsearchClientSettings) : BackgroundService
{
    private const int DayDelay = 30;
    /// <summary>
    /// Thực thi dịch vụ cập nhật dữ liệu dân tộc.
    /// </summary>
    /// <param name="stoppingToken">Token để dừng tác vụ.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            bool success;
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
                success = false;
                sw.Stop();
                logger.Error(ex,$"Khởi tạo dữ liệu dân tộc thất bại: [{ex.Message}][{sw.Elapsed.TotalSeconds}s]");
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
                logger.Information(
                    $"Lần đồng bộ dữ liệu dân tộc tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                logger.Warning(
                    $"Sẽ thực hiện lại cập nhật dữ liệu dân tộc lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}"); 
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    /// <summary>
    /// Đồng bộ dữ liệu dân tộc.
    /// </summary>
    /// <returns>
    /// Trả về thông báo kết quả đồng bộ.
    /// </returns>
    private async Task<(string message, bool success)> SyncData()
    {
        string message;
        if (string.IsNullOrEmpty(postgreSqlConnection.PrimaryConnectionString))
        {
            message = "Không tìm thấy chuỗi kết nối cơ sở dữ liệu";
            logger.Error(message);
            return (message, false);
        }
        logger.Information("Bắt đầu khởi tạo dữ liệu dân tộc");
        var danTocSerice = new DanTocService(postgreSqlConnection, elasticsearchClientSettings);
        var (insert, update, skip) = await danTocSerice.UpdateDanTocAsync();
        message = $"Khởi tạo dữ liệu dân tộc thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]";
        return (message, true);
    }
}