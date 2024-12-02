using System.Text;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDa.App.Api.Identity.Endpoints;
using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.Audit.MongoDb;
using Haihv.DatDai.Lib.Extension.Configuration.Elasticsearch;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.WebApp;
using Haihv.DatDai.Lib.Identity.Data.Services;
using Haihv.DatDai.Lib.Identity.DbUp.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Services;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddServiceDefaults();

builder.AddLogToElasticsearch();

// Khởi tạo kết nối PostgreSQL
builder.AddPostgreSqlConnection();

// Đăng ký dịch vụ cập nhật cấu trúc cơ sở dữ liệu
builder.Services.AddSingleton<DataBaseInitializer>();

// Khởi tạo kết nối Elasticsearch
builder.AddElasticsearchClient();

// Khỏi tạo Audit to MongoDB
builder.UseAuditDatToMongoDb();

builder.Services.AddOpenApi();

// Add service for LDAP
var ldapSection = builder.Configuration.GetSection("LdapConnectionInfo");
var ldapConnectionInfo = new LdapConnectionInfo()
{
    Host = ldapSection["Host"] ?? string.Empty,
    Port = int.Parse(ldapSection["Port"] ?? "389"),
    Username = ldapSection["PrincipalNameAdmin"] ?? string.Empty,
    Password = ldapSection["PasswordAdmin"] ?? string.Empty,
    RootGroupDn = ldapSection["RootGroupDn"] ?? string.Empty,
    AdminGroupDn = ldapSection["AdminGroupDn"] ?? string.Empty,
    SearchBase = ldapSection["SearchBase"] ?? string.Empty,
    Domain = ldapSection["Domain"] ?? string.Empty,
    DomainFullname = ldapSection["DomainFullname"] ?? string.Empty,
    Organizational = ldapSection["Organizational"] ?? string.Empty
};

builder.Services.AddSingleton<ILdapContext>(new LdapContext(ldapConnectionInfo));
builder.Services.AddSingleton<IUserLdapService,UserLdapService>();

builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddSingleton<IAuthenticateLdapService, AuthenticateLdapService>();

builder.Services.AddSingleton<TokenProvider>();

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
