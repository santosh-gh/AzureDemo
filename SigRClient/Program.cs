using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO;
using Newtonsoft.Json;

namespace SigRClient
{
	class Program
	{
		const string DIR = @"C:\tmp\PollerEventData";

		static void Main(string[] args)
		{
			Directory.CreateDirectory(DIR);

			StreamWriter all = null;
			StreamWriter terminate = null;
			StreamWriter nailupChanged = null;
			StreamWriter callArrived = null;
			StreamWriter callEnded = null;
			StreamWriter wrapChanged = null;
			StreamWriter transferStatusChanged = null;

			if (false)
			{
				all = new StreamWriter($@"{DIR}\all.txt");
				terminate = new StreamWriter($@"{DIR}\terminate.txt");
				nailupChanged = new StreamWriter($@"{DIR}\nailupChanged.txt");
				callArrived = new StreamWriter($@"{DIR}\callArrived.txt");
				callEnded = new StreamWriter($@"{DIR}\callEnded.txt");
				wrapChanged = new StreamWriter($@"{DIR}\wrapChanged.txt");
				transferStatusChanged = new StreamWriter($@"{DIR}\transferStatusChanged.txt");
			}

			try
			{
				var connection = new HubConnectionBuilder()
					.WithUrl("http://localhost:7071/api?userid=sant@gmail.com").Build();

				connection.On<TerminateEventData>("terminate", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"terminate: {json}");
					all?.WriteLine(json);
					terminate?.WriteLine(json);
				});

				connection.On<NailupChangedEventData>("nailupChanged", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"nailupChanged: {json}");
					all?.WriteLine(json);
					nailupChanged?.WriteLine(json);
				});

				connection.On<CallArrivedEventData>("callArrived", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"callArrived: {json}");
					all?.WriteLine(json);
					callArrived?.WriteLine(json);
				});

				connection.On<CallEndedEventData>("callEnded", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"callEnded: {json}");
					all?.WriteLine(json);
					callEnded?.WriteLine(json);
				});

				connection.On<WrapChangedEventData>("wrapChanged", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"wrapChanged: {json}");
					all?.WriteLine(json);
					wrapChanged?.WriteLine(json);
				});

				connection.On<TranserStatusChangedEventData>("transferStatusChanged", data =>
				{
					string json = JsonConvert.SerializeObject(data);

					Console.WriteLine($"transferStatusChanged: {json}");
					all?.WriteLine(json);
					transferStatusChanged?.WriteLine(json);
				});

				Task t = connection.StartAsync();
				t.Wait();

				Console.WriteLine($"Connection Id '{connection.ConnectionId}'");

			}
			catch(Exception ex)
			{
				var x = ex.Message;
			}

			

			

			Console.ReadLine();

			all?.Dispose();
			terminate?.Dispose();
			nailupChanged?.Dispose();
			callArrived?.Dispose();
			callEnded?.Dispose();
			wrapChanged?.Dispose();
			transferStatusChanged?.Dispose();
		}
	}
}
