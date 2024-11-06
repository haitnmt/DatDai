namespace Haihv.DatDai.Lib.Data.Base;

/// <summary>
/// Lớp chứa các thiết lập cho ứng dụng.
/// </summary>
public static class Settings
{
    /// <summary>
    /// Tính toán thời gian trễ cho việc đồng bộ dữ liệu.
    /// </summary>
    /// <param name="day">Số ngày trễ.</param>
    /// <param name="hour">Số giờ trễ.</param>
    /// <param name="minute">Số phút trễ.</param>
    /// <param name="seconds">Số giây trễ.</param>
    /// <param name="isRandom">Có bổ sung thời gian ngẫu nhiên hay không.</param>
    /// <returns>Thời gian trễ dưới dạng <see cref="TimeSpan"/>.</returns>
    public static (TimeSpan Delay, DateTime NextSyncTime) GetDelayTime(int day = 1, int hour = 0, int minute = 0, int seconds = 0, bool isRandom = true)
    {
        // Lên lịch đồng bộ lại dữ liệu vào khoảng thời gian 0h-6h sau DayDelay ngày
        var now = DateTime.Now;
        var nextSyncTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0)
            .AddDays(day)
            .AddHours(hour)
            .AddMinutes(minute)
            .AddSeconds(seconds);
        if (now.Hour < 6)
        {
            nextSyncTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
        }
        // Bổ sung random thời gian đồng bộ dữ liệu
        var random = new Random();
        nextSyncTime = nextSyncTime.AddSeconds(isRandom ? random.Next(0, 7200) : 0);
        return (nextSyncTime - now, nextSyncTime);
    }
}