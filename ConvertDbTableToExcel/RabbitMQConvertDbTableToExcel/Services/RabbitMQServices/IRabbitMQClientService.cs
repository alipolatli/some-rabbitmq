using RabbitMQ.Client;

namespace RabbitMQConvertDbTableToExcel.Services.RabbitMQServices
{
    public interface IRabbitMQClientService
    {
        string ExchangeName { get; }
        string RoutingKey { get; }
        string QueueName { get; }

        /// <summary>
        /// Exchange, kuyruk,bind işlemlerini,konfigürasyonları yapar.
        /// </summary>
        /// <returns>
        /// Publish etmeye hazır channel döner.
        /// </returns>
        IModel ConnectRabbitMQ();
    }
}
