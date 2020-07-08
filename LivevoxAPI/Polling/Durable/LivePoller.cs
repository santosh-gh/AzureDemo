using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace LivevoxAPI.Durable
{
    public static class LivePoller
    {
        public class LivePollerRequestBody : ClientRequestBody
        {
            public string EventUrl { get; set; }
            public string UserId { get; set; }
        }

        public class LivePollerStart
        {
            public string EventUrl { get; set; }
            public string BaseUrl { get; set; }
            public string AccessToken { get; set; }
            public string SessionId { get; set; }
            public string UserId { get; set; }
        }       

        [FunctionName("LivePoller")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // get the request body passed to the http start
            LivePollerRequestBody body = context.GetInput<LivePollerRequestBody>();

            // use the request body to get info about the portal
            var portalInfo = await context.CallActivityAsync<PortalInfo>("GetPortalInfo", body.PortalName);

            // create a poller start object
            var start = new LivePollerStart()
            {
                EventUrl = body.EventUrl,
                BaseUrl = portalInfo.BaseUrl,
                AccessToken = portalInfo.AccessToken,
                SessionId = body.SessionId,
                UserId = body.UserId
            };

            // start the poller activity
            var result = await context.CallActivityAsync<int>("LivePoller_Run", start);
            return result;
        }

        [FunctionName("GetPortalInfo")]
        public static PortalInfo GetPortalInfo([ActivityTrigger] string portalName, ILogger log)
        {
            return Portal.GetPortalInfo(portalName);
        }

        [FunctionName("LivePoller_Run")]
        public static int Run([ActivityTrigger] LivePollerStart start, ILogger log)
        {
            Polling.IPollerConnect livevox = new Polling.LiveLivevox(
              start.BaseUrl, start.AccessToken, start.SessionId, log);

            Polling.Poller.Run(livevox, start.EventUrl, start.UserId, log);

            return 0;
        }

        [FunctionName("LivePoller_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            LivePollerRequestBody start = await req.Content.ReadAsAsync<LivePollerRequestBody>();

            start.EventUrl = string.Format("{0}://{1}:{2}/api/messages",
              req.RequestUri.Scheme, req.RequestUri.Host, req.RequestUri.Port);

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("LivePoller", start);

            string statusUrl = string.Format("{0}://{1}:{2}/api/LivePollerStatus?id={3}",
              req.RequestUri.Scheme, req.RequestUri.Host, req.RequestUri.Port, instanceId);

            string terminateUrl = string.Format("{0}://{1}:{2}/api/LivePollerTerminate?id={3}",
              req.RequestUri.Scheme, req.RequestUri.Host, req.RequestUri.Port, instanceId);

            log.LogInformation($"Started LivePoller with ID = '{instanceId}'.");

            //return starter.CreateCheckStatusResponse(req, instanceId);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            response.Headers.Add("StatusUrl", statusUrl);
            response.Headers.Add("TerminateUrl", terminateUrl);

            // Update Redis Cache 
            IDatabase cache = Dialer.RedisStore.Connection.GetDatabase();
            UserSession Existingdata = JsonConvert.DeserializeObject<UserSession>(cache.StringGet(start.SessionId));
            if (Existingdata != null)
            {
                UserSession session = new UserSession(Existingdata.clientName, Existingdata.userId, Existingdata.sessionId, instanceId);
                // cache.KeyDelete(sessionId);
                cache.StringSet(start.SessionId, JsonConvert.SerializeObject(session));
            }

            return response;
        }


        [FunctionName("LivePollerStatus")]
        public static async Task<IActionResult> LivePollerStatus(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req,
          [DurableClient]IDurableOrchestrationClient orchestrationClient,
          ILogger log)
        {
            var orchestrationInstanceId = req.Query["id"];

            if (string.IsNullOrWhiteSpace(orchestrationInstanceId))
            {
                return new NotFoundResult();
            }

            // Get the status for the passed in instanceId
            DurableOrchestrationStatus status = await orchestrationClient.GetStatusAsync(orchestrationInstanceId);

            if (status is null)
            {
                return new NotFoundResult();
            }

            var shortStatus = new
            {
                currentStatus = status.RuntimeStatus.ToString(),
                result = status.Output
            };

            return new OkObjectResult(shortStatus);
        }

        [FunctionName("LivePollerTerminate")]
        public static async Task<IActionResult> LivePollerTerminate(
          [HttpTrigger(AuthorizationLevel.Anonymous, "delete")]HttpRequest req,
          [DurableClient]IDurableOrchestrationClient orchestrationClient,
          ILogger log)
        {
            var orchestrationInstanceId = req.Query["id"];

            if (string.IsNullOrWhiteSpace(orchestrationInstanceId))
            {
                return new NotFoundResult();
            }

            await orchestrationClient.TerminateAsync(orchestrationInstanceId, "ttl=0");
            return new OkResult();
        }
    }
}