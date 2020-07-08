using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Configuration;

namespace LivevoxAPI
{
    public static class Function1
    {
        [FunctionName("RedisCacheTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            //var connString = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
            //var cache = ConnectionMultiplexer.Connect(connString).GetDatabase();


            var connString = "livevox-redis.redis.cache.windows.net:6379,password=QXfVfokhTLBHEqg0pi5XuW2ixbmFm54TUlGS2ERVUs4=,ssl=False,abortConnect=False";
            var redis = ConnectionMultiplexer.Connect(connString);
            IDatabase cache = redis.GetDatabase();         
            try
            {
                // TODO - insert your code here to manipulate data in your Redis cache
                cache.StringSet("one", "one-value");
                var cashedata = cache.StringGet("one");
                Console.WriteLine("Cache response : " + cache.StringGet("one").ToString());
            }
            catch (Exception ex)
            {

            }

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
