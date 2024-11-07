using Haihv.DatDai.Data.Identity;
using Haihv.DatDai.Identity.InLdap;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.

// Add service for LDAP
var rootLdapKey = builder.Configuration.GetSection("LdapConnectionInfo");
var ldapConnectionInfo = new LdapConnectionInfo()
{
    Host = builder.Configuration[$"{rootLdapKey}:Host"] ?? "localhost",
    Port = int.Parse(builder.Configuration[$"{rootLdapKey}:Port"] ?? "389"),
    Username =  builder.Configuration[$"{rootLdapKey}:PrincipalNameAdmin"] ?? "Administrator",
    Password = builder.Configuration[$"{rootLdapKey}:PasswordAdmin"] ?? string.Empty,
    RootGroupDn = builder.Configuration[$"{rootLdapKey}:RootGroupDn"] ?? "cn=Administrators",
    AdminGroupDn = builder.Configuration[$"{rootLdapKey}:AdminGroupDn"] ?? "cn=Administrators",
    SearchBase = builder.Configuration[$"{rootLdapKey}:SearchBase"] ?? "dc=example,dc=com",
    Domain = builder.Configuration[$"{rootLdapKey}:Domain"] ?? "example",
    DomainFullname = builder.Configuration[$"{rootLdapKey}:DomainFullname"] ?? "example.com",
    Organizational = builder.Configuration[$"{rootLdapKey}:Organizational"] ?? "ou=Users"
};
builder.Services.AddSingleton<ILdapContext>(new LdapContext(ldapConnectionInfo));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// Add service for IdentityDbContext
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

/*var summaries = new[]
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
    .WithName("GetWeatherForecast");*/

app.Run();

/*namespace Haihv.DatDai.IdentityApi
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}*/