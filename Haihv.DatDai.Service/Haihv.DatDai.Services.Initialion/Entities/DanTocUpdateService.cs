using System.Text;
using Haihv.DatDai.Data.Base;
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
                await SyncAdministrativeUnits();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thất bại [{ex.Message}] [{ex}]");
            }
            var (delay, nextSyncTime) = Settings.GetDelayTime(day: DayDelay);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Lần đồng bộ dữ liệu dân tộc tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SyncAdministrativeUnits()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu khởi tạo dữ liệu dân tộc");
        var service = new DanTocSerice(new DanhMucDbContext(options));
        var (insert, update, skip) = await service.UpdateDvhcAsync();
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Khởi tạo dữ liệu dân tộc thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]");
    }
    
}