using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace LivevoxAPI.Durable
{
    public static class FilePoller
    {
        public class Start
        {
            public string File { get; set; }
            public bool Fast { get; set; }
            public string EventUrl { get; set; }
            public string SignalRid { get; set; }
        }

        [FunctionName("FilePoller")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Start body = context.GetInput<Start>();

            var result = await context.CallActivityAsync<int>("FilePoller_Run", body);


            return result;
        }

        [FunctionName("FilePoller_Run")]
        public static int Run([ActivityTrigger] Start start, ILogger log)
        {
            Polling.IPollerConnect livevox = new Polling.FileLivevox(start.File, start.Fast);
            Polling.Poller.Run(livevox, start.EventUrl, start.SignalRid, log);
            return 0;
        }

        [FunctionName("FilePoller_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            var query = req.RequestUri.ParseQueryString();
            Start start = new Start()
            {
                File = @$"C:\tmp\livevox\{query["file"]}",
                Fast = (query["fast"] ?? "false") == "true",
                SignalRid = query["sigr"] ?? String.Empty,
                EventUrl = string.Format("{0}://{1}:{2}/api/messages",
                req.RequestUri.Scheme, req.RequestUri.Host, req.RequestUri.Port)
            };

            if (String.IsNullOrEmpty(start.SignalRid))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("FilePoller", start);

            log.LogInformation($"Started FilePoller with ID = '{instanceId}'.");

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        //[FunctionName("FilePoller")]
        //public static async Task<List<string>> RunOrchestrator(
        //    [OrchestrationTrigger] IDurableOrchestrationContext context)
        //{
        //    var outputs = new List<string>();

        //    // Replace "hello" with the name of your Durable Activity Function.
        //    outputs.Add(await context.CallActivityAsync<string>("FilePoller_Hello", "Tokyo"));
        //    outputs.Add(await context.CallActivityAsync<string>("FilePoller_Hello", "Seattle"));
        //    outputs.Add(await context.CallActivityAsync<string>("FilePoller_Hello", "London"));

        //    // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        //    return outputs;
        //}

        //[FunctionName("FilePoller_Hello")]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation($"Saying hello to {name}.");
        //    return $"Hello {name}!";
        //}

        //[FunctionName("FilePoller_HttpStart")]
        //public static async Task<HttpResponseMessage> HttpStart(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
        //    [DurableClient] IDurableOrchestrationClient starter,
        //    ILogger log)
        //{
        //    // Function input comes from the request content.
        //    string instanceId = await starter.StartNewAsync("FilePoller", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}
    }
}