using haihv.DatDai.Data.DanhMuc.Services;
using haihv.DatDai.Services.SyncDhvc;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();