using System.Diagnostics;
using System.Reflection;
using DbUp;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.DbUp.PostgreSQL;

public class DataBaseInitializer(PostgreSqlConnection postgreSqlConnection, ILogger logger)
{
    private readonly string _connectionString = postgreSqlConnection.PrimaryConnectionString;
    public Task InitializeAsync()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            logger.Error("Connection string is empty");
            return Task.CompletedTask;
        }
        var sw = Stopwatch.StartNew();
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();
        var result = upgrader.PerformUpgrade();
        sw.Stop();
        if (!result.Successful)
        {
            logger.Error(result.Error,$"Cập nhật cấu trúc dữ liệu thất bại: {result.Error.Message}, [{sw.Elapsed.TotalSeconds}s]");
        }
        logger.Information($"Cập nhật cấu trúc dữ liệu thành công [{sw.Elapsed.TotalSeconds}s]");
        return Task.CompletedTask;
    }
}