using RabbitMQ.Client;

namespace RabbitMQAddWatermarksToImages.Services
{
    public class RabbitMQClientService : IRabbitMQClientService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitMQClientService> _logger;

        public string ExchangeName => "watermak-direct-exchange";

        public string RoutingKey => "watermark-route";

        public string QueueName => "watermark-queue";

        public RabbitMQClientService(IConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();
            CreateChannel();
            _logger = logger;
        }

        public IModel ConnectRabbitMQ()
        {
            _channel.ExchangeDeclare(exchange: ExchangeName,
                                     type: ExchangeType.Direct,
                                     durable: true,
                                     autoDelete: false,
                                     arguments: null);

            _channel.QueueDeclare(queue: QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueBind(queue: QueueName,
                                exchange: ExchangeName,
                                routingKey: RoutingKey,
                                arguments: null);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu.");

            return _channel;
        }

        /// <summary>
        /// Halihazırda channel ayakta ise yeni channel oluşturmaz. Yoksa oluşturur.
        /// </summary>
        private void CreateChannel()
        {
            if (_channel is { IsOpen: true })
            {
                return;
            }
            _channel = _connection.CreateModel();
        }

        /// <summary>
        /// Tüm bağlantıları kapatır ve dispose eder.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _channel = default;
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı kapatıldı.");
        }
    }
}
