using System.Text;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;


/// <summary>
/// Dịch vụ cập nhật dữ liệu quốc tịch.
/// </summary>
/// <param name="options">Tùy chọn cho DbContext của QuocTich.</param>
/// <param name="mongoDbContext">Ngữ cảnh MongoDB.</param>
/// <param name="memoryCache">Bộ nhớ đệm.</param>
public class QuocTichUpdateService(
    DbContextOptions<QuocTichDbContext> options,
    IMongoDbContext mongoDbContext,
    IMemoryCache memoryCache) : BackgroundService
{
    private const int DayDelay = 30;
    private readonly LogSystemrRepository _logSystemrRepository = new(mongoDbContext);

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
            var log = await _logSystemrRepository.UpdateAsync(new LogSystemEntry()
            {
                SystemName = "QuocTichUpdateService",
                SystemDescription = "Dịch vụ cập nhật dữ liệu quốc tịch",
                FunctionName = "ExecuteAsync",
                Metadata = "Cập nhật dữ liệu quốc tịch"
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
                log.Exception = ex.Message;
                log.Success = false;
                log.EndTimeUtc = DateTime.UtcNow;
                await _logSystemrRepository.UpdateAsync(log);
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thất bại [{ex.Message}] [{ex}]");
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Lần Cập nhật dữ liệu quốc tịch tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Sẽ thực hiện lại cập nhật dữ liệu quốc tịch lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}");
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
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu cập nhật dữ liệu quốc tịch");
        var service = new RestCountriesService(options, mongoDbContext, memoryCache);
        var (insert, update, skip) = await service.UpdateAsync();
        var message = $"Cập nhật dữ liệu quốc tịch thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]";
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {message}");
        return message;
    }
}