using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQExchangeTypesFanout.Consumer
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
                //**fanout exchange kullanımında genellikle kuyruk consumer tarafında oluşturulur.
                channel.QueueBind(queue: queueName,
                                  exchange: "custom-fanout-exchange",
                                  routingKey: ""
                                  );

                //TEK SEFERDE ALINACAK MESAJ SAYISI
                channel.BasicQos(prefetchSize: 0,// mesaj boyutu
                                 prefetchCount: 1,//kaç kaç geleceği
                                 global: false);//true olursa count adeti tek seferde kuyruklara böler. Örn 9 mesaj 3 consumer'e 3 3 3 olarak paylaştırılır. false olursa tek seferde 9 mesaj 1 consumer'e gönderilir.

                Console.WriteLine("Consumer dinlemede, henüz mesaj gelmedi.");

                //EVENT İLE MESAJI YAKALAMA
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
                {
                    string message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine($" Kuyruktan {e.Body} tipinde gelen mesaj \n string'e dönüştürülerek değeri '{message}' olarak belirlenmiştir.");
                    channel.BasicAck(deliveryTag: e.DeliveryTag,//kuyruk içindeki verinin işlendiği bilgisini iletir ve öyle siler.(autoack false)
                                    multiple: true);//true olursa ramde işlenmiş ama rabbitmqya gitmemiş başka mesajlar varsa onların da haberini verir.
                };
                //GELEN MESAJIN KONFİGURASYONU
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,//true olursa kuyruktaki veri consumer'a gönderildikten sonra kuyruk içindeki veri silinir. false olursa silinmez.
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}