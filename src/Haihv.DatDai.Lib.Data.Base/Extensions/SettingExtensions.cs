namespace Haihv.DatDai.Lib.Data.Base.Extensions;

/// <summary>
/// Lớp chứa các thiết lập cho ứng dụng.
/// </summary>
public static class SettingExtensions
{
    /// <summary>
    /// Tính toán thời gian trễ cho việc đồng bộ dữ liệu.
    /// </summary>
    /// <param name="days">Số ngày trễ.</param>
    /// <param name="hours">Số giờ trễ.</param>
    /// <param name="minutes">Số phút trễ.</param>
    /// <param name="seconds">Số giây trễ.</param>
    /// <param name="isRandom">Bổ sung thời gian ngẫu nhiên</param>
    /// <param name="isMidnightToMorning"><c>true</c> thì thời gian đồng bộ tiếp theo sẽ nằm trong khoảng 0h-6h.
    /// <c>Chỉ thực hiện khi day >=1</c>
    /// </param>
    /// <returns>Thời gian trễ dưới dạng <see cref="TimeSpan"/>.</returns>
    public static (TimeSpan Delay, DateTime NextSyncTime) GetDelayTime(int days = 1, int hours = 0, int minutes = 0,
        int seconds = 0, bool isRandom = true, bool isMidnightToMorning = true)
    {
        // Tạo đối tượng Random để sinh số ngẫu nhiên
        var random = new Random();
        var now = DateTime.Now;
        // Tính toán thời gian trễ với các giá trị ngẫu nhiên nếu cần
        var delay = new TimeSpan(
            (isRandom && days > 0 ? random.Next(days * 12, days * 36) : days * 24) +
            (isRandom && hours > 0 ? random.Next(hours/2, hours*3/2) : hours),
            isRandom && minutes > 0 ? random.Next(minutes/2, minutes*3/2) : minutes,
            isRandom && seconds > 0 ? random.Next(seconds/2, seconds*3/2) : seconds);

        // Tính toán thời gian đồng bộ tiếp theo
        var nextSyncTime = now.Add(delay);
        // Nếu không cần giới hạn thời gian từ 0h-6h hoặc thời gian đồng bộ tiếp theo <= 6h, trả về kết quả
        if (delay.Days < 1 || 
            nextSyncTime.Hour <= 6 || 
            !isMidnightToMorning) return (delay, nextSyncTime);

        // Điều chỉnh thời gian đồng bộ tiếp theo để nằm trong khoảng 0h-6h nếu cần
        nextSyncTime = nextSyncTime.AddHours(random.Next(0, 6));
        delay = nextSyncTime - now;

        // Trả về thời gian trễ và thời gian đồng bộ tiếp theo
        return (delay, nextSyncTime);
    }
}