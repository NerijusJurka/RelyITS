using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using RelyITS;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace RelyITS
{
	public class Program
	{
		public static void Main()
		{
			var DailyTime = "13:00:00"; //Time to execute code
			var timeParts = DailyTime.Split(new char[1] { ':' });
			while (true)
			{
				var dateNow = DateTime.Now;
				var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
				int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
				TimeSpan ts;
				if (date > dateNow)
				ts = date - dateNow;
				else
				{
					date = date.AddDays(1);
					ts = date - dateNow;
				}
				Console.WriteLine("Waiting for {0}", DailyTime);
				Task.Delay(ts).Wait();
				var instance = new Consumer();
				instance.ConsumeQueue();
			}
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

