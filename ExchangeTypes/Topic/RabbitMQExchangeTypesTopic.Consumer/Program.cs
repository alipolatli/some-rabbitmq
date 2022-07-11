using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQExchangeTypesTopic.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { Uri = new Uri("amqp://localhost:5672") };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                string queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                 exchange: "custom-topic-exchange",
                                 routingKey:"*.kemal.*"
                                 );
                var consumer = new EventingBasicConsumer(channel);
                Console.WriteLine("Dinleniyor...");
                consumer.Received += (sender, args) =>
                {
                    Console.WriteLine($"Mesaj: {Encoding.UTF8.GetString(args.Body.ToArray())}");
                    channel.BasicAck(args.DeliveryTag, true);
                };
                channel.BasicConsume(queueName, false, consumer);

                Console.ReadLine();
            }
        }
    }
}