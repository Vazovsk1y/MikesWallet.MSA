using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MikesWallet.ExchangeRates.WebApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(e => e.AddDocumentTransformer((x, _, _) =>
{
    x.Servers = [];
    return Task.CompletedTask;
}));

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = builder.Configuration.GetConnectionString("Redis");
    o.InstanceName = "MikesWallet_";
});

builder.Services.AddScoped<IExchangeRatesProvider, RandomExchangeRatesProvider>();
builder.Services.AddHostedService<ExchangeRatesUpdateWorker>();

builder.Services.AddAuthorization();
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
}

app.MapGet("/api/rates", async (IDistributedCache cache, CancellationToken cancellationToken) =>
{
    cancellationToken.ThrowIfCancellationRequested();
    
    var ratesStr = await cache.GetStringAsync("ExchangeRates", cancellationToken);
    if (string.IsNullOrWhiteSpace(ratesStr))
    {
        return Results.NoContent();
    }
    
    var result = JsonSerializer.Deserialize<List<ExchangeRate>>(ratesStr);
    return Results.Ok(result);
}).RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.Run();