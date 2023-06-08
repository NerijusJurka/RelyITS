using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Linq;


namespace RelyITS
{
	public class Program
	{
		public static void Main()
		{
			var consumer = new Consumer();
			var timer = new ConsumerTimer(consumer);
			timer.Start();

			Console.WriteLine("Scheduled job started. Enter 'exit' to stop.");
			while (true)
			{
				var input = Console.ReadLine();
				if (input?.ToLower() == "exit")
					break;
			}
			timer.Stop();
		}
		public static void Send(string queue, string data)
		{
			try
			{
				queue = "SiustiJson";// RabbitMQ send Queue name
				string fileName = @"C:\temp\Receipt.json";
				string json = System.IO.File.ReadAllText(fileName);
				var receiptJson = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReceiptJson>>(json);
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
				Main();
			}
		}
	}
}

