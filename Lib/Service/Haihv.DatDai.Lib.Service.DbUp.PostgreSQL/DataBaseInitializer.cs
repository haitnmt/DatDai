using System.Reflection;
using DbUp;

namespace Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;

public class DataBaseInitializer(string connectionString)
{
    public Task InitializeAsync()
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();
        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            throw new Exception("Database migration failed", result.Error);
        }

        return Task.CompletedTask;
    }
}