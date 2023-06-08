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
	}
}

