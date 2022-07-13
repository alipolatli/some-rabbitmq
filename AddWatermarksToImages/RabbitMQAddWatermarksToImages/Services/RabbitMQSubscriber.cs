using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQAddWatermarksToImages.Services
{
    public class RabbitMQSubscriber
    {
        private readonly IRabbitMQClientService _rabbitMQClientService;
        private IModel _channel;
        readonly ILogger<RabbitMQSubscriber> _logger;

        public RabbitMQSubscriber(IRabbitMQClientService rabbitMQClientService, ILogger<RabbitMQSubscriber> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        public void ConfigConsumer()
        {
            _logger.LogInformation("RabbitMQ bağlantısı açıldı.Consumer'ın kaç mesaj kabul edeceği basicQqs methodu ile belirlendi.");
            _channel = _rabbitMQClientService.ConnectRabbitMQ();
            _channel.BasicQos(0, 1, false);
        }

        public void Consume()
        {
            _logger.LogInformation("Consumer oluşturuldu ve event tetiklendi. Consume edildi.");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(queue: _rabbitMQClientService.QueueName,
                                  autoAck: false,
                                  consumer: consumer);
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            _logger.LogInformation("Event için girildi. Gelen mesaj complex type titipne geri dönştürüldü ve watermark ekleme işlemleri yapıldı.");
            string bodyJson = Encoding.UTF8.GetString(e.Body.ToArray());
            var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(bodyJson);
            try
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productImageCreatedEvent.ImageName);
                using Image image = Image.FromFile(imagePath);
                using Graphics graphic = Graphics.FromImage(image);
                var font = new Font(FontFamily.GenericSerif, 50, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString("my-custom-watermark", font);
                var color = Color.White;
                var brush = new SolidBrush(color);
                var point = new Point(image.Width - ((int)textSize.Width + 30), image.Height - ((int)textSize.Height + 30));
                graphic.DrawString("my-custom-watermark", font, brush, point);

                image.Save("wwwroot/images/watermarks/" + productImageCreatedEvent.ImageName);

                _channel.BasicAck(e.DeliveryTag, false);
            }
            catch(ArgumentNullException nullex)
            {
                _logger.LogError(nullex.Message);
                throw nullex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
