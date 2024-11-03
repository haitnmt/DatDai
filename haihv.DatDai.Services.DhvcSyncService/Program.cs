using haihv.DatDai.Data.DanhMuc.Services;
using haihv.DatDai.Services.SyncDhvc;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateApplicationBuilder(args);

// Configure the DataDaiDbContext
builder.Services.AddDbContext<DanhMucDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<DvhcService>();
builder.Services.AddScoped<CapTinhRepository>();
builder.Services.AddScoped<CapHuyenRepository>();
builder.Services.AddScoped<CapXaRepository>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();