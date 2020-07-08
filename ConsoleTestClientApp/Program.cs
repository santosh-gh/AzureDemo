using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;


namespace ConsoleTestClientApp
{
    class Program
    {
        public class LoginData
        {
            public string clientName { get; set; }
            public string userName { get; set; }
            public string password { get; set; }
            public Boolean agent { get; set; }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Login Users!");

            

            var connection = new HubConnectionBuilder().WithUrl("http://localhost:7071/api").Build();


            connection.On<LoginData>("notify", (message) =>
            {
                Console.WriteLine("Client Name:{0} ", message.clientName);
                Console.WriteLine("User Name:{0} ", message.userName);               
            });


            connection.On("notify", (string message) =>
            {
                Console.WriteLine($"Message from server {message}");
            }
  );
            connection.StartAsync();
            Console.ReadKey();
        }
    }
}
