using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RelyITS
{
    public class MessageSender
    {
        public void Send(string queue, string data)
        {
            try
            {
                queue = "SiustiJson"; // RabbitMQ send Queue name
                string fileName = @"C:\temp\Receipt.json";
                if (!File.Exists(fileName))
                {
                    using (File.Create(fileName)) { }
                }
                string json = File.ReadAllText(fileName);
                var receiptJson = JsonConvert.DeserializeObject<List<ReceiptJson>>(json);
                var descReceiptJson = receiptJson.OrderBy(x => x.StoreID); //Group by StoreID
                json = JsonConvert.SerializeObject(descReceiptJson);
                using (IConnection connection = new ConnectionFactory().CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue, false, false, false, null);
                        channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(json));
                    }
                }
            }
            finally
            {
                Console.WriteLine("Scheduled job is done");
                File.Delete(@"C:\temp\Receipt.json");
            }
        }
    }
}
