using System.Text;
using Haihv.DatDai.Data.DanhMuc.Dvhc.Services;
using Haihv.DatDai.Service.UpdateDvhc.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Service.UpdateDvhc;

public class DvhcUpdateService(DbContextOptions<DvhcDbContext> options) : BackgroundService
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
            // Lên lịch đồng bộ lại dữ liệu vào khoảng thời gian 0h-6h sau DayDelay ngày
            var now = DateTime.Now;
            var nextSyncTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(DayDelay);
            if (now.Hour < 6)
            {
                nextSyncTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            }
            // Bổ sung random thời gian đồng bộ dữ liệu
            var random = new Random();
            nextSyncTime = nextSyncTime.AddSeconds(random.Next(0, 7200));
            var delay = nextSyncTime - now;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: giờ Lần đồng bộ dữ liệu đơn vị hành chính tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SyncAdministrativeUnits()
    {
        var capTinhRepository = new CapTinhEntitiy(options);
        var capHuyenRepository = new CapHuyenEntitiy(options);
        var capXaRepository = new CapXaEntitiy(options);
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
 