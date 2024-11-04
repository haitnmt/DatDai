using System.Text;
using Haihv.DatDai.Data.DanhMuc.Services;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Services.Initialion.Entities;

public class DanTocUpdateService(DbContextOptions<DanhMucDbContext> options) : BackgroundService
{
    private const int DayDelay = 30;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set Console support for Vietnamese
        Console.OutputEncoding = Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu khởi tạo dữ liệu dân tộc");
                await SyncAdministrativeUnits();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thất bại [{ex.Message}] [{ex}]");
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
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Lần đồng bộ dữ liệu dân tộc tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SyncAdministrativeUnits()
    {
        var service = new DanTocSerice(new DanhMucDbContext(options));
        var (insert, update) = await service.UpdateDvhcAsync();
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thành công [Thêm mới: {insert}, Cập nhật: {update}]");
    }
    
}