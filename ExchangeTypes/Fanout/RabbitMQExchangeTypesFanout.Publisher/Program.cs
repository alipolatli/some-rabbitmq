using RabbitMQ.Client;
using System.Text;

namespace RabbitMQExchangeTypesFanout.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672");
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())//kanal oluşumu (publisher => channel=> exchange => queue => channel=> consumer)
            {
                string exchangeName = "custom-fanout-exchange";
                channel.ExchangeDeclare(exchange: exchangeName,
                                        type: ExchangeType.Fanout,
                                        durable: false);

                string message = $"Merhaba, ben bir kuyruk içindeki veriyim.";
                byte[] byteMessage = Encoding.UTF8.GetBytes(message);
                for (int i = 0; i < 50; i++)
                {
                    channel.BasicPublish(exchange: exchangeName,//verileri exchange tipine göre ilgili kuyruğa iletir.(default,fanout,direct,topic,header) belirtilmezse default exchange kategorisne girer.
                                 routingKey: "",//default exchange olursa kuyruk ismi verilir. direct exchange'de durum farklıdır.
                                 basicProperties: null,
                                 body: byteMessage//kuyruğa gönderilecek byte[] tipinden veri.
                                 );
                    Console.WriteLine($"'{message}' isimli mesaj \n byte[{byteMessage.Length}] tipine dönüştürülerek \n'random' isimli kuyruğa \n fanout exchange konfigürasyonu ile başarıyla gönderilmiştir.");
                }



                Console.ReadLine();
            }
        }
    }
}