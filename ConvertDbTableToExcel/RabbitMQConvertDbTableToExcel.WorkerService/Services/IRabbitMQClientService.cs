using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConvertDbTableToExcel.WorkerService.Services
{
    public interface IRabbitMQClientService
    {
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
