using Haihv.DatDai.Data.DanhMuc.Dvhc.Services;
using Haihv.DatDai.Service.UpdateDvhc;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var options = new DbContextOptionsBuilder<DvhcDbContext>()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options;

builder.Services.AddHostedService<DvhcUpdateService>( _ => new DvhcUpdateService(options));
var host = builder.Build();
host.Run();