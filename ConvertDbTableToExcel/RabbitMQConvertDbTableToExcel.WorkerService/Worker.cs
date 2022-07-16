using ClosedXML.Excel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQConvertDbTableToExcel.Shared;
using RabbitMQConvertDbTableToExcel.WorkerService.Models;
using RabbitMQConvertDbTableToExcel.WorkerService.Services;
using System.Data;
using System.Text;
using System.Text.Json;

namespace RabbitMQConvertDbTableToExcel.WorkerService
{
    public class Worker : BackgroundService
    {
        readonly IRabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<Worker> _logger;
        readonly IServiceProvider _serviceProvider;
        IModel _channel;

        public Worker(ILogger<Worker> logger, IRabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ worker backgroound service startasync methodu çalışıyor.");
            _channel = _rabbitMQClientService.ConnectRabbitMQ();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(queue: _rabbitMQClientService.QueueName,
                                  autoAck: false,
                                  consumer: consumer);
            return Task.CompletedTask;
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(e.Body.ToArray()));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XLWorkbook xLWorkbook = new XLWorkbook();
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(GetTable("Products"));

                xLWorkbook.Worksheets.Add(dataSet);
                xLWorkbook.SaveAs(memoryStream);

                ///

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                multipartFormDataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "excelFile", Guid.NewGuid().ToString() + ".xlsx");

                ///
                var apiUrl = $"https://localhost:7093/api/excelfiles?userFileId={createExcelMessage.UserFileId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(apiUrl,multipartFormDataContent);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Excel File : {createExcelMessage.UserFileId} başarılı.");
                        _channel.BasicAck(e.DeliveryTag, false);
                    }
                }
            }


        }

        private DataTable GetTable(string tableName)
        {
            List<Product> products;
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var northWindContext = scope.ServiceProvider.GetRequiredService<NorthwindContext>();

                products = northWindContext.Products.ToList();
            }
            DataTable table = new DataTable(tableName);
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("QuantityPerUnit", typeof(string));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("UnitsInStock", typeof(short));

            foreach (var item in products)
            {
                table.Rows.Add(item.ProductId, item.ProductName, item.QuantityPerUnit, item.UnitPrice, item.UnitsInStock);
            }

            return table;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}