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

namespace LivevoxAPI
{
    public static class RedisCache
    {
        [FunctionName("ConnectRedisCache")]
        public static async Task<IActionResult> ConnectRedisCache(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Dialer.RedisStore.cache = Dialer.RedisStore.Connection.GetDatabase();        
            return new OkObjectResult("OK");
        }
        [FunctionName("DisconnectRedisCache")]
        public static async Task<IActionResult> DisconnectRedisCache(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {          
            Dialer.RedisStore.Connection.Dispose(); 
            return new OkObjectResult("OK");
        }
    }
}
