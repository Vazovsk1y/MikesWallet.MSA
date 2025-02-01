using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MikesWallet.Users.DAL;
using MikesWallet.Users.Infrastructure;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(e => e.AddDocumentTransformer((a, _, _) =>
{
    a.Servers = [];
    return Task.CompletedTask;
}));

builder.Services.AddDbContext<ApplicationDbContext>(e => e.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
builder.Services.AddOptions<AccessTokenSettings>().BindConfiguration("AccessToken");
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

var mainGroup = app.MapGroup("api").AddFluentValidationAutoValidation();

mainGroup.MapAuthEndpoints();
app.Run();