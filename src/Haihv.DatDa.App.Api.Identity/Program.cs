using System.Text;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDa.App.Api.Identity.Endpoints;
using Haihv.DatDa.App.Api.Identity.Entities;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.WebApp;
using Haihv.DatDai.Lib.Identity.Data;
using Haihv.DatDai.Lib.Identity.Ldap;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddServiceDefaults();

builder.AddLogToElasticsearch();

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

builder.Services.AddSingleton<TokenProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// Add service for IdentityDbContext
builder.Services.AddDbContext<IdentityDbContext>(options => { options.UseNpgsql(connectionString); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapLoginEndpoints();

app.MapDefaultEndpoints();

app.Run();