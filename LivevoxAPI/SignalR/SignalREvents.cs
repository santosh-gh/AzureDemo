using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Logging;

namespace LivevoxAPI.SignalR
{
	public static class SignalREvents
	{

        //Read ConnectionID from storage
        //Task<Dialer.SignalRConnInfo> task = Task.Run<Dialer.SignalRConnInfo>(async () => await GetSignalRConnectionID("john"));       

        
        //[FunctionName("negotiate")]
        //public static SignalRConnectionInfo GetOrderNotificationsSignalRInfo(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
        //    [SignalRConnectionInfo(HubName = "notifications")] SignalRConnectionInfo connectionInfo)
        //{
        //    //var a = req.HttpContext.Connection.Id;
        //    //string username = req.Query["username"];

        //    return connectionInfo;
        //}

        //[FunctionName("negotiate")]
        //public static SignalRConnectionInfo Negotiate(
        //[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        //[SignalRConnectionInfo(HubName = "notifications", UserId = "{headers.x-ms-client-principal-id}")]
        //SignalRConnectionInfo connectionInfo)
        //{
        //    var a= req.HttpContext.Connection.
        //    //string username = req.Query["username"];
        //    // connectionInfo contains an access key token with a name identifier claim set to the authenticated user
        //    return connectionInfo;
        //}

        [FunctionName("negotiate")]
        public static async Task<SignalRConnectionInfo> GetSignalRInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        IBinder binder)
        {
            string userId = req.Query["userid"];
            SignalRConnectionInfoAttribute attribute = new SignalRConnectionInfoAttribute
            {
                HubName = "notifications",
                UserId = userId
            };

            SignalRConnectionInfo connection = await binder.BindAsync<SignalRConnectionInfo>(attribute);

            return connection;
        }    

        [FunctionName("messages")]
        public static Task SendMessage(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post")] EventStart data,
           [SignalR(HubName = "notifications")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
            new SignalRMessage
            {
                UserId = data.UserId,
                Target = data.EventName,
                Arguments = new[] { data.EventData }
            }); ; ;
        }

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        internal static HttpStatusCode SendMessage(string url, EventStart eventStart)
        {
          HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
          req.Method = "POST";
          req.ContentType = "application/json";

          using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
          {
            sw.Write(JsonConvert.SerializeObject(eventStart));
          }

          try
          {
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            return rsp.StatusCode;
          }
          catch (WebException ex)
          {
            if (ex.Response == null) return HttpStatusCode.Gone;

            HttpWebResponse rsp = (HttpWebResponse)ex.Response;
            return rsp.StatusCode;
          }
        }

        [FunctionName("SetSignalRConnection")]
        public static async Task<IActionResult> SetSignalRConnection(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
             ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic info = JsonConvert.DeserializeObject(requestBody);

            Dialer.SignalRConnInfo sci = new Dialer.SignalRConnInfo
            {
                UserID = info.UserID,
                ConnectionID = info.ConnectionID
            };

            if (sci.UserID == null || sci.ConnectionID == null)
            {
                return new BadRequestObjectResult("Please specify 'UserID' and 'ConnectionID' in the request body");
            }

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storagedialer;AccountKey=GWNroKYrQsImZQzZ7ur5wlukeyEnoDaL8cP5Bbg+NOQ4r53wk//F1nqyNFyX5cW6PJEYU/P8EJiiFBWq58j7kg==;EndpointSuffix=core.windows.net");
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();

            CloudTable cloudTable = tableClient.GetTableReference("SignalRConnInfo");

            Dialer.SignalRConnInfo.CreateNewTable(cloudTable);

            bool y = await cloudTable.CreateIfNotExistsAsync();

            Task<Dialer.SignalRConnInfo> task = Task.Run<Dialer.SignalRConnInfo>(async () => await Dialer.SignalRConnInfo.RetrieveRecord(cloudTable, "100", sci.UserID));

            Dialer.SignalRConnInfo Recinfo = task.Result;
            if (Recinfo != null)
            {
                Dialer.SignalRConnInfo.UpdateRecordInTable(cloudTable, sci.UserID, sci.ConnectionID);
            }
            else
            {
                Dialer.SignalRConnInfo.InsertRecordToTable(cloudTable, sci.UserID, sci.ConnectionID);
            }
            
            return new OkObjectResult("Success");
        }

        private static async Task<Dialer.SignalRConnInfo> GetSignalRConnectionID(string userId)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storagedialer;AccountKey=GWNroKYrQsImZQzZ7ur5wlukeyEnoDaL8cP5Bbg+NOQ4r53wk//F1nqyNFyX5cW6PJEYU/P8EJiiFBWq58j7kg==;EndpointSuffix=core.windows.net");
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();

            CloudTable cloudTable = tableClient.GetTableReference("SignalRConnInfo");

            TableOperation tableOperation = TableOperation.Retrieve<Dialer.SignalRConnInfo>("100", userId);
            TableResult tableResult = await cloudTable.ExecuteAsync(tableOperation);  

            return tableResult.Result as Dialer.SignalRConnInfo;
        }       
    }
}
