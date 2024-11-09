using LanguageExt.ClassInstances.Const;
using Npgsql;
using StackExchange.Redis;

namespace Haihv.DatDai.Lib.Data.Base;

public interface INpgsqlDataConnectionService
{
    Task UpdateAsync(params string[] connectionStrings);
    Task<string> GetConnectionStringAsync(bool isPrimary = true);
}

public class NpgsqlDataConnectionService(string redisConnectionString, int defaultDatabase = 0) : INpgsqlDataConnectionService
{
    private const string PrimaryKey = $"NpgsqlConnection:Primary";
    private const string SecondaryKey = $"NpgsqlConnection:Secondary";
    private static string GetSecondaryKey(int key) => $"{SecondaryKey}:{key}";
    
    public async Task UpdateAsync(params string[] connectionStrings)
    {
        await UpdateAsync(redisConnectionString, defaultDatabase, connectionStrings);
    }
    public async Task<string> GetConnectionStringAsync(bool isPrimary = true)
    {
        return await GetConnectionStringAsync(redisConnectionString, defaultDatabase, isPrimary);
    }

    private static async Task UpdateAsync(string redisConnectionString, int defaultDatabase = 0, params string[] connectionStrings)
    {
        var redisConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisConfigurationOptions.DefaultDatabase = defaultDatabase;
        var redis = await ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions);
        var redisDatabase = redis.GetDatabase();
        var index = 0;
        foreach (var connectionString in connectionStrings.Distinct())
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine($"Connection string is empty");
            }
            else
            {
                var (hasWritePermission, error) = await CheckWritePermission(connectionString);
                if (hasWritePermission)
                    await redisDatabase.StringSetAsync(PrimaryKey, connectionString);
                else
                {
                    if (!string.IsNullOrWhiteSpace(error)) continue;
                    await redisDatabase.StringSetAsync(GetSecondaryKey(index), connectionString);
                    index++;
                }
            }
        }
        var count = await redisDatabase.StringLengthAsync(SecondaryKey);
        for (var i = index + 1; i < count; i++)
        {
            await redisDatabase.KeyDeleteAsync(GetSecondaryKey(i));
        }
    }

    private static async Task<string> GetConnectionStringAsync(string redisConnectionString, int defaultDatabase = 0, bool isPrimary = true)
    {
        var redisConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisConfigurationOptions.DefaultDatabase = defaultDatabase;
        var redis = await ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions);
        var redisDatabase = redis.GetDatabase();
        if (isPrimary) return (await redisDatabase.StringGetAsync(PrimaryKey)).ToString();
        // Lấy tổng số chuỗi kết nối sắn có:
        var exist = true;
        var count = 0;
        while (exist)
        {
            var key = GetSecondaryKey(count);
            if (await redisDatabase.KeyExistsAsync(key))
            {
                count++;
            }
            else
            {
                exist = false;
            }
        }

        return count switch
        {
            0 => (await redisDatabase.StringGetAsync(PrimaryKey)).ToString(),
            1 => (await redisDatabase.StringGetAsync(GetSecondaryKey(0))).ToString(),
            _ => (await redisDatabase.StringGetAsync(GetSecondaryKey(new Random().Next(count)))).ToString()
        };
    }
    
    // Kiểm tra xem chuỗi kết nối có quyền ghi dữ liệu lên tất cả các bảng hay không?
    private static async Task<(bool, string error) > CheckWritePermission(string connectionString)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, ex.Message);
            }
        
            // Lấy username từ connection string
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            await using var command = connection.CreateCommand();
            // Kiểm tra quyền INSERT trên tất cả các bảng trong schema public
            const string commandText = "INSERT INTO _checkstatus (\"Id\", \"CheckedAt\") VALUES (@id, @checkedAt)";
            command.CommandText = commandText;
            command.Parameters.AddWithValue("@id",  Guid.CreateVersion7());
            command.Parameters.AddWithValue("@checkedAt", DateTimeOffset.UtcNow);
            try
            {
                await command.ExecuteNonQueryAsync();
                // Xóa trạng thái trước đó 1 ngày:
                command.Parameters.Clear();
                const string deleteCommandText = "DELETE FROM _checkstatus WHERE \"CheckedAt\" < @checkedAt";
                command.CommandText = deleteCommandText;
                var date = DateTimeOffset.UtcNow.AddDays(-1);
                command.Parameters.AddWithValue("@checkedAt", date);
                var res = await command.ExecuteNonQueryAsync();
                
                // Trả về true nếu không có lỗi
                return (true, string.Empty);
            }
            catch
            {
                return (false, string.Empty);
            }
        }
        catch 
        {
            return (false, string.Empty);
        }
    }
}