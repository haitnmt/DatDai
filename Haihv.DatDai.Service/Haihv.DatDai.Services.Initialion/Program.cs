using Haihv.DatDai.Data.DanhMuc.Services;
using Haihv.DatDai.Service.UpdateDvhc;
using Haihv.DatDai.Service.UpdateQuocTich;
using Haihv.DatDai.Services.Initialion.Entities;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var options = new DbContextOptionsBuilder<DanhMucDbContext>()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options;

var optionsQt = new DbContextOptionsBuilder<QuocTichDbContext>()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options;

builder.Services.AddHostedService<DvhcUpdateService>( _ => new DvhcUpdateService(options));
builder.Services.AddHostedService<DanTocUpdateService>( _ => new DanTocUpdateService(options));
builder.Services.AddHostedService<QuocTichUpdateService>( _ => new QuocTichUpdateService(optionsQt));
var host = builder.Build();
host.Run();
