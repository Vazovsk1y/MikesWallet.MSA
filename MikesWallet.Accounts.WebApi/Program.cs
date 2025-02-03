using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MikesWallet.Accounts.WebApi.DAL;
using MikesWallet.Accounts.WebApi.DAL.Models;
using MikesWallet.Accounts.WebApi.Infrastructure;
using MikesWallet.Contracts.Cache;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(e => e.AddDocumentTransformer((x, _, _) =>
{
    x.Servers = [];
    return Task.CompletedTask;
})
.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services.AddDbContext<ApplicationDbContext>(e => e.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = builder.Configuration.GetConnectionString("Redis");
    o.InstanceName = "MikesWallet_";
});

builder.Services.AddAuthentication(e =>
    {
        e.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AccessToken:Issuer"],
            ValidAudience = builder.Configuration["AccessToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AccessToken:SecretKey"]!)),
            ClockSkew = TimeSpan.Parse(builder.Configuration["AccessToken:ClockSkew"]!),
            AuthenticationType = JwtBearerDefaults.AuthenticationScheme,
        };

        options.MapInboundClaims = false;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

var cache = app.Services.GetRequiredService<IDistributedCache>();
var currenciesString = await cache.GetStringAsync("Currencies");
if (string.IsNullOrWhiteSpace(currenciesString))
{
    var cacheEntry = Enum
        .GetValues<Currency>()
        .Select(e => new CurrencyCacheModel{ AlphabeticCode = e.ToString() })
        .ToList();
    
    await cache.SetStringAsync("Currencies", JsonSerializer.Serialize(cacheEntry));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();