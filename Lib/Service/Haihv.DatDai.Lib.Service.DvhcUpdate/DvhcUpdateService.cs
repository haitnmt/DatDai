using System.Text;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Service.DvhcUpdate.Entities;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Service.DvhcUpdate;

/// <summary>
/// Dịch vụ cập nhật dữ liệu đơn vị hành chính.
/// </summary>
public class DvhcUpdateService : BackgroundService
{
    private readonly INpgsqlDataConnectionService _npgsqlDataConnectionService;
    private const int DayDelay = 1;
    private readonly LogSystemrRepository _logSystemrRepository;
    private readonly IMongoDbContext _mongoDbContext;

    public DvhcUpdateService(
        INpgsqlDataConnectionService npgsqlDataConnectionService,
        IMongoDbContext mongoDbContext)
    {
        _npgsqlDataConnectionService = npgsqlDataConnectionService;
        _mongoDbContext = mongoDbContext;
        _logSystemrRepository = new LogSystemrRepository(mongoDbContext);
    }
    
    public DvhcUpdateService(string redisConnectionInfo, int redisDatabase, IMongoDbContext mongoDbContext)
    {
        _npgsqlDataConnectionService = new NpgsqlDataConnectionService(redisConnectionInfo, redisDatabase);
        _mongoDbContext = mongoDbContext;
        _logSystemrRepository = new LogSystemrRepository(mongoDbContext);
    }
    
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
            var log = await _logSystemrRepository.UpdateAsync(new LogSystemEntry()
            {
                SystemName = "DvhcUpdateService",
                SystemDescription = "Dịch vụ cập nhật dữ liệu đơn vị hành chính",
                FunctionName = "ExecuteAsync",
                Metadata = "Đồng bộ dữ liệu đơn vị hành chính"
            });
            var success = true;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu đồng bộ dữ liệu đơn vị hành chính");
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
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Đồng bộ dữ liệu đơn vị hành chính thất bại [{ex.Message}] [{ex}]");
                log.Exception = ex.Message;
                log.Success = false;
                log.EndTimeUtc = DateTime.UtcNow;
                await _logSystemrRepository.UpdateAsync(log);
            }

            var delay = TimeSpan.FromMinutes(5);
            if (success)
            {
                (delay, var nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Đồng bộ dữ liệu đơn vị hành chính thành công");
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Lần đồng bộ dữ liệu đơn vị hành chính tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                Console.WriteLine(
                    $"{DateTime.Now:HH:mm:ss}: Sẽ thực hiện lại đồng bộ dữ liệu đơn vị hành chính lúc: {DateTime.Now.AddMinutes(5):dd/MM/yyyy HH:mm:ss}");
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
    private async Task<string> SyncData()
    {
        var capTinhRepository = new CapTinhEntitiy(_npgsqlDataConnectionService, _mongoDbContext);
        var capHuyenRepository = new CapHuyenEntitiy(_npgsqlDataConnectionService, _mongoDbContext);
        var capXaRepository = new CapXaEntitiy(_npgsqlDataConnectionService, _mongoDbContext);
        // Xử lý đồng thời 3 cấp hành chính
        var tasks = new List<Task<string>>
        {
            capTinhRepository.CreateOrUpdateAsync(),
            capHuyenRepository.CreateOrUpdateAsync(),
            capXaRepository.CreateOrUpdateAsync()
        };
        var messages = await Task.WhenAll(tasks);
        return string.Join(", ", messages);
    }
}