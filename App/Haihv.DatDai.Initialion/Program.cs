using Haihv.DatDai.Lib.Data.DanhMuc;
using Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Service.QuocTichUpdate;
using Haihv.DatDai.Lib.Service.DvhcUpdate;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Services.Initialion.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var options = new DbContextOptionsBuilder<DanhMucDbContext>()
    .UseNpgsql(connectionString)
    .Options;

var optionsQt = new DbContextOptionsBuilder<QuocTichDbContext>()
    .UseNpgsql(connectionString)
    .Options;

var mongoDbConnectionInfo = new MongoDbConnectionInfo(builder.Configuration.GetConnectionString("MongoDbConnection")!,
    builder.Configuration.GetConnectionString("MongoDbDatabaseName")!);

builder.Services.AddKeyedScoped<List<AuditEntry>>("Audit", (_,_) => []);

builder.Services.AddSingleton(_ => new DataBaseInitializer(connectionString));


// Khởi tạo các dịch vụ cập nhật dữ liệu
builder.Services.AddHostedService<DvhcUpdateService>( _ => new DvhcUpdateService(options, 
    new MongoDbContext(mongoDbConnectionInfo), new MemoryCache(new MemoryCacheOptions())));
builder.Services.AddHostedService<DanTocUpdateService>( _ => new DanTocUpdateService(options,
    new MongoDbContext(mongoDbConnectionInfo), new MemoryCache(new MemoryCacheOptions())));
builder.Services.AddHostedService<QuocTichUpdateService>( _ => new QuocTichUpdateService(optionsQt,
    new MongoDbContext(mongoDbConnectionInfo), new MemoryCache(new MemoryCacheOptions())));
var host = builder.Build();

// Khởi tạo cơ sở dữ liệu
var dataBaseInitializer = host.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();


host.Run();
