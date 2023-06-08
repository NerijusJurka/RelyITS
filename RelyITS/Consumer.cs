using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RelyITS
{
    public class Consumer
    {
        public void ConsumeQueue()
        {
            string url = "amqp://guest:guest@localhost/%2f";
            var factory = new ConnectionFactory { Uri = new Uri(url) };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Receipt", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var processor = new JsonProcessor();
                    processor.ReadAndWriteToJson(message);
                };

                channel.BasicConsume(queue: "Receipt", autoAck: true, consumer: consumer);

                Console.WriteLine("Scheduled job started");
            }
        }
    }
}