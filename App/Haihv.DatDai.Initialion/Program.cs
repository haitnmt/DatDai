using Haihv.DatDai.Lib.Data.DanhMuc.Services;
using Haihv.DatDai.Lib.Service.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Service.QuocTichUpdate;
using Haihv.DatDai.Lib.Service.DvhcUpdate;
using Haihv.DatDai.Services.Initialion.Entities;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var options = new DbContextOptionsBuilder<DanhMucDbContext>()
    .UseNpgsql(connectionString)
    .Options;

var optionsQt = new DbContextOptionsBuilder<QuocTichDbContext>()
    .UseNpgsql(connectionString)
    .Options;

builder.Services.AddSingleton(_ => new DataBaseInitializer(connectionString));

builder.Services.AddHostedService<DvhcUpdateService>( _ => new DvhcUpdateService(options));
builder.Services.AddHostedService<DanTocUpdateService>( _ => new DanTocUpdateService(options));
builder.Services.AddHostedService<QuocTichUpdateService>( _ => new QuocTichUpdateService(optionsQt));
var host = builder.Build();
var dataBaseInitializer = host.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();
host.Run();
