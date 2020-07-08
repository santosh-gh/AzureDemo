using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
namespace LivevoxAPI
{
    public static class SignalRAPI
    {
        [FunctionName("negotiate1")]
        public static SignalRConnectionInfo GetOrderNotificationsSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost)] HttpRequest req,
            [SignalRConnectionInfo(HubName = Dialer.Global.SignalRHubName)] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("messages1")]
        public static Task SendMessage(
           [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost)] LoginData message,
           [SignalR(HubName = Dialer.Global.SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = Dialer.Global.MessageTarget,
                    Arguments = new[] { message }
                });
        }
    }
}
