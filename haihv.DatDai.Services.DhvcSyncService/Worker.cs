using haihv.DatDai.Data.DanhMuc.Services;
using Microsoft.EntityFrameworkCore;

namespace haihv.DatDai.Services.SyncDhvc;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private const string Url = "https://danhmuchanhchinh.gso.gov.vn/DMDVHC.asmx";
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24); // Configurable sync interval

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Bắt đầu đồng bộ dữ liệu từ {Url} lúc: {Time}", Url, DateTimeOffset.Now);
                await SyncAdministrativeUnits();
                logger.LogInformation("Hoàn thành đồng bộ dữ liệu lúc: {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi trong quá trình đồng bộ dữ liệu: {Message}", ex.Message);
            }

            try
            {
                await Task.Delay(_syncInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Dịch vụ đồng bộ đã được dừng lại");
                break;
            }
        }
    }

    private async Task SyncAdministrativeUnits()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DanhMucDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        await using var dbContext = new DanhMucDbContext(optionsBuilder.Options);
        var capTinhRepository = new CapTinhRepository(dbContext);
        var capHuyenRepository = new CapHuyenRepository(dbContext);
        var capXaRepository = new CapXaRepository(dbContext);

        // Xử lý tuần tự
        await capTinhRepository.CreateOrUpdateAsync();
        await capHuyenRepository.CreateOrUpdateAsync();
        await capXaRepository.CreateOrUpdateAsync();
    }
}
