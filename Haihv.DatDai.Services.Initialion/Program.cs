using Haihv.DatDai.Data.DanhMuc.Services;
using Haihv.DatDai.Service.UpdateDvhc;
using Haihv.DatDai.Services.Initialion.Entities;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var options = new DbContextOptionsBuilder<DanhMucDbContext>()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options;

//builder.Services.AddHostedService<DvhcUpdateService>( _ => new DvhcUpdateService(options));
builder.Services.AddHostedService<DanTocUpdateService>( _ => new DanTocUpdateService(options));
var host = builder.Build();
host.Run();
