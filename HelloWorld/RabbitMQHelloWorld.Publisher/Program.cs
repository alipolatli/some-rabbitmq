using RabbitMQ.Client;
using System.Text;

namespace RabbitMQHelloWorld.Publisher
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
                string queueName = "hello_world_queue";
                channel.QueueDeclare(queue: queueName,//kuyruk ismi
                                     durable: true,//kuyruklar ram'de tutulur. rabbitMQ'ya restart atılırsa tüm kuyruklar silinir. true ile fiziksel olarak tutulması sağlanır ve kaybın önüne geçilir.
                                     exclusive: false,//true olursa yalnızca publisherda oluşturulan kanal üzerinden kuyruğa bağlantı sağlanabilir. false olursa farklı kanallardan da(consumer) kuyruğa bağlanılabilir. 
                                     autoDelete: false,//kuyruğa bağlı consumerlar işini bitirdiği takdirde kuyruk otomatik olarak silinir. 
                                     arguments: null
                                     );


                for (int i = 0; i < 50; i++)
                {
                    string message = $"Merhaba, ben bir kuyruk içindeki {i}. veriyim.";
                    byte[] byteMessage = Encoding.UTF8.GetBytes(message);


                    channel.BasicPublish(exchange: String.Empty,//verileri exchange tipine göre ilgili kuyruğa iletir.(default,fanout,direct,topic,header) belirtilmezse default exchange kategorisne girer.
                                     routingKey: queueName,//default exchange olursa kuyruk ismi verilir. direct exchange'de durum farklıdır.
                                     basicProperties: null,
                                     body: byteMessage//kuyruğa gönderilecek byte[] tipinden veri.
                                     );
                    Console.WriteLine($"'{message}' isimli mesaj \n byte[{byteMessage.Length}] tipine dönüştürülerek \n {queueName} isimli kuyruğa \n default exchange konfigürasyonu ile başarıyla gönderilmiştir.");

                }

                Console.ReadLine();
            }
        }
    }
}