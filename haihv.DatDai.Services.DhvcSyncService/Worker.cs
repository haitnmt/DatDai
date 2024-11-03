namespace haihv.DatDai.Services.SyncDhvc;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
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
        using var scope = serviceProvider.CreateScope();
        var capTinhRepository = scope.ServiceProvider.GetRequiredService<CapTinhRepository>();
        var capHuyenRepository = scope.ServiceProvider.GetRequiredService<CapHuyenRepository>();
        var capXaRepository = scope.ServiceProvider.GetRequiredService<CapXaRepository>();
        var tasks = new List<Task>
        {
            capTinhRepository.CreateOrUpdateAsync(),
            capHuyenRepository.CreateOrUpdateAsync(),
            capXaRepository.CreateOrUpdateAsync()
        };

        await Task.WhenAll(tasks);
    }
}
