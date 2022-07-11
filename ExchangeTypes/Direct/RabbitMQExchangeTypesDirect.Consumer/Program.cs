using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQExchangeTypesDirect.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { Uri = new Uri("amqp://localhost:5672") };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                var queueName = $"direct-queue-Critical";

                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) =>
                {
                    var message = Encoding.UTF8.GetString(args.Body.ToArray());
                    Thread.Sleep(500);
                    Console.WriteLine($" Kuyruktan {args.Body} tipinde gelen mesaj \n string'e dönüştürülerek değeri '{message}' olarak belirlenmiştir.");
                    channel.BasicAck(args.DeliveryTag,false);
                    File.AppendAllText("log-info.txt", message +"\n");

                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}