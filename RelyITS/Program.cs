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
		public static void ReadAndWriteToJson(string message)
		{
				var deserializedObject = DeserializeFromXML(message);
				using (StreamWriter w = File.AppendText(@"C:\temp\Receipt.json")) { };//create temporary json file
				var filePath = @"C:\temp\Receipt.json";
				var jsonData = System.IO.File.ReadAllText(filePath);
				var receiptList = JsonConvert.DeserializeObject<List<ReceiptJson>>(jsonData) ?? new List<ReceiptJson>();
				receiptList.Add(new ReceiptJson()
				{
					Date = DateTime.Now.Date,
					StoreID = deserializedObject.transaction.UnitIDs[0].UnitID,
					TotalItems = deserializedObject.transaction.retailTransactions[0].Total,
					TotalAmount = deserializedObject.transaction.retailTransactions[0].lineItem.Sale.Quantity,
					TotalReceipts = 1
					
				});
				var instance = new DatabaseConnection();
				instance.Connection(deserializedObject.transaction.SequenceNumber,deserializedObject.transaction.UnitIDs[0].UnitID, deserializedObject.transaction.retailTransactions[0].lineItem.Sale.Quantity, deserializedObject.transaction.retailTransactions[0].Total);
				jsonData = JsonConvert.SerializeObject(receiptList);
				System.IO.File.WriteAllText(filePath, jsonData);
				
		}
		public static Structure.POSLog DeserializeFromXML(string message)
		{
			var serializer = new XmlSerializer(typeof(Structure.POSLog));
			Structure.POSLog result;
			using (TextReader reader = new StringReader(message))
			{
				result = (Structure.POSLog)serializer.Deserialize(reader);
			}
			return result;

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

