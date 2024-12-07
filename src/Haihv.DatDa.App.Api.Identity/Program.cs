using System.Text;
using Audit.Core;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDa.App.Api.Identity.Endpoints;
using Haihv.DatDai.Lib.Extension.Audit.MongoDb;
using Haihv.DatDai.Lib.Extension.Configuration.Elasticsearch;
using Haihv.DatDai.Lib.Extension.Configuration.Ldap;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.WebApp;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Data.Services;
using Haihv.DatDai.Lib.Identity.Data.Services.Background;
using Haihv.DatDai.Lib.Identity.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Ldap.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Microsoft.Extensions.Caching.Hybrid;
using Scalar.AspNetCore;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddServiceDefaults();

builder.AddLogToElasticsearch();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});
// Đăng ký MemoryCache
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache(option =>
{
    option.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromDays(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
});
#pragma warning restore EXTEXP0018

// Khởi tạo kết nối PostgreSQL
builder.AddPostgreSqlConnection();
// Add service for LDAP
builder.AddLdapContext();

// Đăng ký dịch vụ cập nhật cấu trúc cơ sở dữ liệu
builder.Services.AddSingleton<DataBaseInitializer>();

// Khởi tạo kết nối Elasticsearch
builder.AddElasticsearchClient();

// Khỏi tạo Audit to MongoDB
builder.UseAuditDatToMongoDb();

builder.Services.AddOpenApi();

// Đăng ký các dịch vụ LDAP
builder.Services.AddSingleton<IUserLdapService,UserLdapService>();
builder.Services.AddSingleton<IGroupLdapService,GroupLdapService>();

// Đăng ký các dịch vụ cơ sở dữ liệu
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IGroupService, GroupService>();

builder.Services.AddSingleton(
    _ => new TokenProvider(builder.Configuration["Jwt:SecretKey"]!,
        builder.Configuration["Jwt:Issuer"]!,
        builder.Configuration["Jwt:Audience"]!,
        builder.Configuration.GetValue<int>("Jwt:ExpireMinutes")));

builder.Services.AddSingleton<IRefreshTokensService>(provider =>
    new RefreshTokensService(provider.GetRequiredService<ILogger>(),
        provider.GetRequiredService<PostgreSqlConnection>(),
        provider.GetRequiredService<AuditDataProvider>(),
        builder.Configuration.GetValue<int>("Jwt:ExpireRefreshTokenDays")));

builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();

// Đăng ký các dịch vụ đồng bộ dữ liệu:
builder.Services.AddHostedService<GroupSyncService>();
builder.Services.AddHostedService<UserSyncService>();
builder.Services.AddHostedService<RevokedRefreshTokensService>();

var app = builder.Build();
// Thực hiện cập nhật cấu trúc cơ sở dữ liệu
var dataBaseInitializer = app.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapLoginEndpoints();

app.MapDefaultEndpoints();

app.Run();
