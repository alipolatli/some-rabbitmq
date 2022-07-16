using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConvertDbTableToExcel.WorkerService.Services
{
    public class RabbitMQClientService : IRabbitMQClientService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitMQClientService> _logger;

        public string QueueName => "excel-queue";

        public RabbitMQClientService(IConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();
            CreateChannel();
            _logger = logger;
        }

        public IModel ConnectRabbitMQ()
        {

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
