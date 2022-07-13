using RabbitMQ.Client;

namespace RabbitMQAddWatermarksToImages.Services
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
