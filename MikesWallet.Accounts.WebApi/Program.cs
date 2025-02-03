using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MikesWallet.Accounts.WebApi.DAL;
using MikesWallet.Accounts.WebApi.Infrastructure;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();