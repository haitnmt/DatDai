using Haihv.DatDai.Lib.Data.Base.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Haihv.DatDai.Lib.Identity.Data.Services.Background;

public class RevokedRefreshTokensService(ILogger logger, IRefreshTokensService refreshTokensService) : BackgroundService
{
    private const int DefaultSecondDelay = 180;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var errorCount = 0;
            var sw = Stopwatch.StartNew();
            (TimeSpan Delay, DateTime NextSyncTime) delayTime;
            try
            {
                var count = await refreshTokensService.DeleteExpiredTokens();
                delayTime = SettingExtensions.GetDelayTime(days: 0, seconds: DefaultSecondDelay);
                sw.Stop();
                logger.Debug(
                    "Xóa các RefreshToken đã hết hạn thành công [{count} bản ghi] [{Elapsed} ms]",
                    count, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                errorCount++;
                delayTime = SettingExtensions.GetDelayTime(days: 0, seconds: 30*errorCount);
                sw.Stop();
                logger.Error(ex,  "Lỗi khi xóa các RefreshToken đã hết hạn [{Elapsed} ms]", sw.ElapsedMilliseconds);
            }
            logger.Debug(
                "Xóa các RefreshToken đã hết hạn lần tiếp theo vào lúc: {NextTime:dd:MM:yyyy HH:mm:ss zz}",
                delayTime.NextSyncTime);
            await Task.Delay(delayTime.Delay, stoppingToken);
        }
    }
}