using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Net;
using StackExchange.Redis;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace LivevoxAPI
{   
    public static class SessionAPI
    {     

        [FunctionName("Login")]
        public static async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
            [SignalR(HubName = Dialer.Global.SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");            
                                 
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            LoginData ld = new LoginData();
            ld.clientName = data.clientName;
            ld.userName = data.userName;
            ld.password = data.password;
            ld.agent = true;

            if (ld.clientName == null || ld.userName == null || ld.password == null)
            {
                return new BadRequestObjectResult("Please specify 'clientName', 'userName' and 'Password' in the request body");
            }

            string json = JsonConvert.SerializeObject(ld);
            string url = "session/login";
            // string accessToken = req.Headers["LV-Access"].FirstOrDefault();
            string accessToken = Dialer.Global.Accesstoken;
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), json,
                accessToken, "123",
                out jsonResponse);
                       
            //Notifiy to client a usr has loged in
            //Need to remobve it only added for testing purpose
            //string x = await InvokeClientNotificationAPI("https://apim-dialer.azure-api.net/livevoxapiapp/messages1", json);

            if (status == HttpStatusCode.Unauthorized)
            { 
                return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); 
            }
            else if (status == HttpStatusCode.NoContent)
            { 
                return (ActionResult)new NoContentResult(); 
            }
            else if (status == HttpStatusCode.BadRequest)
            { 
                return (ActionResult)new BadRequestResult(); 
            }
            else if (status == HttpStatusCode.OK)
            {                                        
                try
                {
                    // insert data to Redis cache                
                    IDatabase cache = Dialer.RedisStore.Connection.GetDatabase();
                    var session = JsonConvert.DeserializeObject<UserSession>(jsonResponse.ToString());
                    UserSession us = new UserSession(ld.clientName, session.userId, session.sessionId, null);
                    cache.StringSet(session.sessionId, JsonConvert.SerializeObject(us));
                }
                catch (Exception ex)
                {
                    log.LogInformation("Exception in Redis Cache...");
                }             
               
                return (ActionResult)new OkObjectResult($"{jsonResponse}");              
            }
            else 
            {
                return (ActionResult)new BadRequestResult(); 
            }
        }   
   
        [FunctionName("Logout")]
        public static async Task<IActionResult> Logout(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbDelete, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            // string sessionId = req.Query["sessionId"];

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string url = "session/logout/" + sessionId;

            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbDelete, url,accessToken,sessionId);

            //return (ActionResult)new NoContentResult();

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "DELETE",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);

            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            {
                //  Delete the Redis instance 
                IDatabase cache = Dialer.RedisStore.Connection.GetDatabase();
                UserSession Existingdata = JsonConvert.DeserializeObject<UserSession>(cache.StringGet(sessionId));
                if (Existingdata != null)
                {
                    cache.KeyDelete(sessionId);
                }
                return (ActionResult)new NoContentResult();
            }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }

        [FunctionName("ChangePassword")]
        public static async Task<IActionResult> ChangePassword(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            ChangePassword  cp = new ChangePassword();
            cp.clientName = data.clientName;
            cp.userName = data.userName;
            cp.password = data.password;
            cp.newPassword = data.newPassword;

            if (cp.clientName == null || cp.userName == null || cp.password == null ||
              cp.newPassword == null )
            {
                return new BadRequestObjectResult("Please specify 'clientName', 'userName', 'password' and 'newPassword' in the request body");
            }

            string json = JsonConvert.SerializeObject(cp);
            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string url = "session/password";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), json,
                accessToken, sessionId,
                out jsonResponse);

            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            { return (ActionResult)new NoContentResult(); }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }
        
        public static async Task<string> InvokeClientNotificationAPI(string url, string json)
        {
            using (var client = new HttpClient())
            {
                //Dictionary<string, string> dictionary = new Dictionary<string, string>();
                //dictionary.Add("PARAM1", "VALUE1");
                //dictionary.Add("PARAM2", text);

                var requestData = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(String.Format(url), requestData);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }       
    }
}
