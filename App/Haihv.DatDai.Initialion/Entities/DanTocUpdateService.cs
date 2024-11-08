using System.Text;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc;
using Haihv.DatDai.Lib.Data.DanhMuc.Services;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Services.Initialion.Entities;

/// <summary>
/// Dịch vụ cập nhật dữ liệu dân tộc.
/// </summary>
/// <param name="options">Tùy chọn DbContext cho DanhMucDbContext.</param>
/// <param name="mongoDbContext">Ngữ cảnh MongoDB.</param>
/// <param name="memoryCache">Bộ nhớ đệm.</param>
public class DanTocUpdateService(
    DbContextOptions<DanhMucDbContext> options,
    IMongoDbContext mongoDbContext,
    IMemoryCache memoryCache) : BackgroundService
{
    private const int DayDelay = 30;
    private readonly LogSystemrRepository _logSystemrRepository = new(mongoDbContext);

    /// <summary>
    /// Thực thi dịch vụ cập nhật dữ liệu dân tộc.
    /// </summary>
    /// <param name="stoppingToken">Token để dừng tác vụ.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Thiết lập hỗ trợ Console cho tiếng Việt
        Console.OutputEncoding = Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            var log = await _logSystemrRepository.UpdateAsync(new LogSystemEntry()
            {
                SystemName = "DanTocUpdateService",
                SystemDescription = "Dịch vụ cập nhật dữ liệu dân tộc",
                FunctionName = "ExecuteAsync",
                Metadata = "Khởi tạo dữ liệu dân tộc"
            });
            var success = true;
            try
            {
                var message = await SyncData();
                log.EndTimeUtc = DateTime.UtcNow;
                log.Success = true;
                log.Metadata = message;
                await _logSystemrRepository.UpdateAsync(log);
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thất bại [{ex.Message}] [{ex}]");
                log.Exception = ex.Message;
                log.Success = false;
                log.EndTimeUtc = DateTime.UtcNow;
                await _logSystemrRepository.UpdateAsync(log);
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Lần đồng bộ dữ liệu dân tộc tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Sẽ thực hiện lại cập nhật dữ liệu dân tộc lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}");
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
    private async Task<string> SyncData()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu khởi tạo dữ liệu dân tộc");
        var service = new DanTocSerice(new DanhMucDbContext(options, mongoDbContext, memoryCache));
        var (insert, update, skip) = await service.UpdateDvhcAsync();
        var message = $"Khởi tạo dữ liệu dân tộc thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]";
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {message}");
        return message;
    }
}