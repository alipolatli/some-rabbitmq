using RabbitMQ.Client;
using System.Text;

namespace RabbitMQExchangeTypesDirect.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { Uri = new Uri("amqp://localhost:5672") };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                string exchangeName = "my-direct-exchange";
                channel.ExchangeDeclare(exchange: exchangeName,
                                        type: ExchangeType.Direct,
                                        durable: true,
                                        autoDelete: true);
                Console.WriteLine("exchange declare edildi.");

                var enums = Enum.GetNames(typeof(LogTypes)).ToList();

                LogTypes logType = (LogTypes)new Random().Next(1, 4);
                string message = $"Ben {logType} tipinde bir log mesajıyım.";
                byte[] byteMessage = Encoding.UTF8.GetBytes(message);
                string queueName = String.Empty;

                foreach (var item in enums)
                {
                    queueName = $"direct-queue-{item}";
                    channel.QueueDeclare(queue: $"direct-queue-{item}",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false);

                    channel.QueueBind(queue: $"direct-queue-{item}",
                        exchange: exchangeName,
                        routingKey: $"route-{item}");
                    Console.WriteLine($"{item} tipinde kuyruk oluştu. Exchange ile bind edildi.");
                }

                for (int i = 0; i < 50; i++)
                {
                    channel.BasicPublish(exchange: exchangeName,
                       routingKey: $"route-{logType}",
                       basicProperties: null,
                       body: byteMessage);

                    Console.WriteLine($"'{message}' isimli mesaj \n byte[{byteMessage.Length}] tipine dönüştürülerek \n kuyruğa direct exchange konfigürasyonu ile başarıyla gönderilmiştir.");
                }
            }

        }
    }

    public enum LogTypes
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Critical = 3
    }
}