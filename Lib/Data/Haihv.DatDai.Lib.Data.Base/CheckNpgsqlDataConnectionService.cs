using Microsoft.Extensions.Hosting;

namespace Haihv.DatDai.Lib.Data.Base;

public class CheckNpgsqlDataConnectionService: BackgroundService
{
    private readonly INpgsqlDataConnectionService _npgsqlDataConnectionService;
    private readonly string[] _connectionStrings;

    public CheckNpgsqlDataConnectionService(
        string redisConnectionString,
        int defaultDatabase = 0,
        params string[] connectionStrings)
    {
        _npgsqlDataConnectionService = new NpgsqlDataConnectionService(redisConnectionString, defaultDatabase);
        _connectionStrings = connectionStrings;
    }
    
    public CheckNpgsqlDataConnectionService(
        INpgsqlDataConnectionService npgsqlDataConnectionService,
        params string[] connectionStrings)
    {
        _npgsqlDataConnectionService = npgsqlDataConnectionService;
        _connectionStrings = connectionStrings;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _npgsqlDataConnectionService.UpdateAsync(_connectionStrings);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}