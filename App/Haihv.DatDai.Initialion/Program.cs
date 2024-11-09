using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Service.QuocTichUpdate;
using Haihv.DatDai.Lib.Service.DvhcUpdate;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Services.Initialion.Entities;
using Microsoft.Extensions.Caching.Memory;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

var readConnectionString = builder.Configuration.GetConnectionString("ReadOnlyConnection")!;


var mongoDbConnectionInfo = new MongoDbConnectionInfo(builder.Configuration.GetConnectionString("MongoDbConnection")!,
    builder.Configuration.GetConnectionString("MongoDbDatabaseName")!);

var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection")!;
var redisDatabase = int.Parse(builder.Configuration.GetConnectionString("RedisDatabase")?? "0");

var memoryCache = new MemoryCache(new MemoryCacheOptions());

builder.Services.AddSingleton<IMemoryCache>(memoryCache);

builder.Services.AddSingleton(_ => new DataBaseInitializer(connectionString));

// Khởi tạo các dịch vụ cập nhật dữ liệu
var npgsqlDataConnectionService =
    new NpgsqlDataConnectionService(redisConnectionString, redisDatabase);

// Khởi tạo dịch vụ kiểm tra kết nối dữ liệu
builder.Services.AddHostedService<CheckNpgsqlDataConnectionService>( _ =>
    new CheckNpgsqlDataConnectionService(npgsqlDataConnectionService, connectionString, readConnectionString));

IMongoDbContext mongoDbContext = new MongoDbContext(mongoDbConnectionInfo);

builder.Services.AddHostedService<DvhcUpdateService>(_ => 
    new DvhcUpdateService(npgsqlDataConnectionService,
        mongoDbContext));
builder.Services.AddHostedService<DanTocUpdateService>( _ => 
    new DanTocUpdateService(npgsqlDataConnectionService,
        mongoDbContext));
builder.Services.AddHostedService<QuocTichUpdateService>( _ => 
    new QuocTichUpdateService(npgsqlDataConnectionService,
        mongoDbContext));
var host = builder.Build();

// Khởi tạo cơ sở dữ liệu
var dataBaseInitializer = host.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();
await npgsqlDataConnectionService.UpdateAsync(connectionString, readConnectionString);

host.Run();
