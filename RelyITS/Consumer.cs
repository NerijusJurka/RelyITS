using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RelyITS
{
    public class Consumer
    {
        public void ConsumeQueue()
        {
            try
            {
                string _url = "amqp://guest:guest@localhost/%2f";
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_url)
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Receipt",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var instance = new Program();
                        Program.ReadAndWriteToJson(message);
                    };
                    channel.BasicConsume(queue: "Receipt",
                        autoAck: true,
                        consumer: consumer);
                    Console.WriteLine("Scheduled job started");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                    
                }
            
            }   
                finally
            {

                Program.Send(" "," ");
            }
        }                
    }      
}
