using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQConvertDbTableToExcel.WorkerService;
using RabbitMQConvertDbTableToExcel.WorkerService.Models;
using RabbitMQConvertDbTableToExcel.WorkerService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, serviceCollection) =>
    {
        IConfiguration configuration = hostBuilderContext.Configuration;
        serviceCollection.AddDbContext<NorthwindContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServer")));
        serviceCollection.AddSingleton<IConnectionFactory>(serviceProvider => new ConnectionFactory { Uri = new Uri(configuration.GetConnectionString("RabbitMQ")) });
        serviceCollection.AddSingleton<IRabbitMQClientService, RabbitMQClientService>();
        serviceCollection.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
