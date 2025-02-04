using System.Reflection;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(e =>
{
    e.SetKebabCaseEndpointNameFormatter();

    e.AddConsumers(Assembly.GetExecutingAssembly());
    
    e.UsingRabbitMq((ctx, c) =>
    {
        c.Host(builder.Configuration.GetConnectionString("RabbitMq")!, h =>
        {
            h.Username("admin");
            h.Password("admin");
        });
        
        c.ConfigureEndpoints(ctx);
    });
});

var host = builder.Build();
host.Run();