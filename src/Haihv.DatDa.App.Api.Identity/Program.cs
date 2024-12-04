using System.Text;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDa.App.Api.Identity.Endpoints;
using Haihv.DatDa.App.Api.Identity.Entities;
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

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddServiceDefaults();

builder.AddLogToElasticsearch();

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

// Đăng ký các dịch vụ Xác thực Người dùng
builder.Services.AddSingleton<IAuthenticateLdapService, AuthenticateLdapService>();
builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();
builder.Services.AddSingleton<TokenProvider>();

// Đăng ký các dịch vụ đồng bộ dữ liệu:
builder.Services.AddHostedService<GroupSyncService>();
builder.Services.AddHostedService<UserSyncService>();


var app = builder.Build();
// Thực hiện cập nhật cấu trúc cơ sở dữ liệu
var dataBaseInitializer = app.Services.GetRequiredService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapLoginEndpoints();

app.MapDefaultEndpoints();

app.Run();
