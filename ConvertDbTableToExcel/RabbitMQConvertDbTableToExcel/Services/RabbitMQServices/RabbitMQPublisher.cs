using RabbitMQConvertDbTableToExcel.Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQConvertDbTableToExcel.Services.RabbitMQServices
{
    public class RabbitMQPublisher
    {
        IRabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(IRabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel= _rabbitMQClientService.ConnectRabbitMQ();

            byte[] bodyByte = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(createExcelMessage));

            channel.BasicPublish(exchange: _rabbitMQClientService.ExchangeName,
                                routingKey: _rabbitMQClientService.RoutingKey,
                                mandatory: false,
                                basicProperties: null,
                                body: bodyByte);
        }
    }
}
