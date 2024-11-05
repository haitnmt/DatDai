using System.Text;
using Haihv.DatDai.Data.Base;
using Haihv.DatDai.Data.DanhMuc.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Service.UpdateQuocTich;

public class QuocTichUpdateService(DbContextOptions<QuocTichDbContext> options)  : BackgroundService
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
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Lần Cập nhật dữ liệu quốc tịch tiếp theo lúc: {nextSyncTime:dd/MM/yyyy HH:mm:ss}");
            await Task.Delay(delay, stoppingToken);
        }
    }
    private async Task SyncAdministrativeUnits()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Bắt đầu cập nhật dữ liệu quốc tịch");
        var service = new RestCountriesService(options);
        var (insert, update, skip) = await service.UpdateAsync();
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Cập nhật dữ liệu quốc tịch thành công [Thêm mới: {insert}, Cập nhật: {update}, Bỏ qua: {skip}]");
    }
}