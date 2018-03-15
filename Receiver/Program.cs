using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Information("Starting Receiver...");

            var services = new ServiceCollection();

            services.AddSingleton(context => Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host(new Uri("rabbitmq://guest:guest@localhost:5672/test"), h => { });

                for (var i = 0; i < 50; i++)
                {
                    x.ReceiveEndpoint(host, $"receiver_queue{i}", e =>
                    {
                        e.Consumer<TestHandler>();
                    });
                }

                x.UseSerilog();
            }));

            var container = services.BuildServiceProvider();

            var busControl = container.GetRequiredService<IBusControl>();

            busControl.Start();

            Log.Information("Receiver started...");
        }
    }
}