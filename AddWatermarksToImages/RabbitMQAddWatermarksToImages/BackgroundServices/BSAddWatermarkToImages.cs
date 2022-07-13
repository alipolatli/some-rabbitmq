using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQAddWatermarksToImages.Services;
using System.Text;
using System.Text.Json;

namespace RabbitMQAddWatermarksToImages.BackgroundServices
{
    public class BSAddWatermarkToImages : BackgroundService
    {
        readonly IRabbitMQClientService _rabbitMQClientService;
        readonly ILogger<BSAddWatermarkToImages> _logger;
        IModel _channel;

        public BSAddWatermarkToImages(IRabbitMQClientService rabbitMQClientService, ILogger<BSAddWatermarkToImages> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start async çalıştı ve rabbitMQ bağlantısı açıldı.Consumer'ın kaç mesaj kabul edeceği basicQqs methodu ile belirlendi.");
            _channel = _rabbitMQClientService.ConnectRabbitMQ();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
      
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consumer oluşturuldu ve event tetiklendi. Consume edildi.");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(queue: _rabbitMQClientService.QueueName,
                                  autoAck: false,
                                  consumer: consumer);
            return Task.CompletedTask;
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            _logger.LogInformation("Event için girildi. Gelen mesaj complex type titipne geri dönştürüldü ve watermark ekleme işlemleri yapıldı.");
            _channel.BasicAck(e.DeliveryTag, true);
            string bodyJson = Encoding.UTF8.GetString(e.Body.ToArray());
            var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(bodyJson);
            Console.WriteLine($"Gelen veri: {productImageCreatedEvent.ImageName}");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync çalıştı.");
            return base.StopAsync(cancellationToken);
        }
    }
}
