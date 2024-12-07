using System.Text;
using Haihv.DatDai.App.Api.DiaChinh;
using Haihv.DatDai.Aspire.ServiceDefault;
using Haihv.DatDai.Lib.Extension.Audit.MongoDb;
using Haihv.DatDai.Lib.Extension.Configuration.Elasticsearch;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.Logger.Elasticsearch.WebApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
builder.AddServiceDefaults();

builder.AddLogToElasticsearch();

// Khởi tạo kết nối PostgreSQL
builder.AddPostgreSqlConnection();

// Khởi tạo kết nối Elasticsearch
builder.AddElasticsearchClient();

// Khỏi tạo Audit to MongoDB
builder.UseAuditDatToMongoDb();

// Add service Authentication and Authorization for Identity Server
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
        };
    });

// Lấy thông tin kết nối CSDL:
var postgreSqlConnection = builder.GetPostgreSqlConnectionString();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .RequireAuthorization()
    .WithName("GetWeatherForecast")
    .WithOpenApi();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.Run();


namespace Haihv.DatDai.App.Api.DiaChinh
{
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}