using RabbitMQ.Client;
using System.Text;

namespace RabbitMQExchangeTypesTopic.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { Uri = new Uri("amqp://localhost:5672") };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "custom-topic-exchange",
                                        type: ExchangeType.Topic,
                                        durable:true,
                                        autoDelete:false);
                for (int i = 0; i < 10; i++)
                {
                    channel.BasicPublish("custom-topic-exchange", "*.kemal.hasan", body: Encoding.UTF8.GetBytes("denemedir"));
                    Console.WriteLine("Mesaj gönderildi.");
                }

                Console.ReadLine();
            }
        }

    }
}