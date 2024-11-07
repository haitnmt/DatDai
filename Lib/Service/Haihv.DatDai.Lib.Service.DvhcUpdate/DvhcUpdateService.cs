using System.Text;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc;
using Haihv.DatDai.Lib.Service.DvhcUpdate.Entities;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Service.DvhcUpdate;

public class DvhcUpdateService(DbContextOptions<DanhMucDbContext> options, IMongoDbContext mongoDbContext) : BackgroundService
{
    private const int DayDelay = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set Console support for Vietnamese
        Console.OutputEncoding = Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu đồng bộ dữ liệu đơn vị hành chính");
                await SyncAdministrativeUnits();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Đồng bộ dữ liệu đơn vị hành chính thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Đồng bộ dữ liệu đơn vị hành chính thất bại [{ex.Message}] [{ex}]");
            }
            var (delay, nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Lần đồng bộ dữ liệu đơn vị hành chính tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SyncAdministrativeUnits()
    {
        var capTinhRepository = new CapTinhEntitiy(options, mongoDbContext);
        var capHuyenRepository = new CapHuyenEntitiy(options, mongoDbContext);
        var capXaRepository = new CapXaEntitiy(options, mongoDbContext);
        // Xử lý đồng thời 3 cấp hành chính
        var tasks = new List<Task>
        {
            capTinhRepository.CreateOrUpdateAsync(),
            capHuyenRepository.CreateOrUpdateAsync(),
            capXaRepository.CreateOrUpdateAsync()
        };
        await Task.WhenAll(tasks);
    }
}
 