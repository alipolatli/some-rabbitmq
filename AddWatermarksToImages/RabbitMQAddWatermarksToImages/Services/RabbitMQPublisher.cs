using System.Text;
using System.Text.Json;

namespace RabbitMQAddWatermarksToImages.Services
{
    public class RabbitMQPublisher
    {
        private readonly IRabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(IRabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitMQClientService.ConnectRabbitMQ();

            string bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            byte[] bodyByte = Encoding.UTF8.GetBytes(bodyString);

            channel.BasicPublish(_rabbitMQClientService.ExchangeName, _rabbitMQClientService.RoutingKey, false, null, bodyByte);

        }
    }
}
