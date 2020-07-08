using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LivevoxAPI.Durable;
using StackExchange.Redis;

namespace LivevoxAPI
{
    public static class LivevoxService
    {
        //[FunctionName("ChangeToNotReady")] // it is not used in our flow
        //public static async Task<IActionResult> ChangeToNotReady(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("ChangeToNotReady function processed a request.");

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    string reasonCode = data?.reasonCode;

        //    if (reasonCode == null)
        //    {
        //        return new BadRequestObjectResult("Please specify 'reasonCode' in the request body");
        //    }

        //    //string responseText = await LivevoxAPI.InvokeLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/status/ready", reasonCode);


        //    return (reasonCode != null)
        //      ? (ActionResult)new OkObjectResult($"{"Text"}")
        //      : new BadRequestObjectResult("Please pass reasonCode in the request body");
        //}

        [FunctionName("ChangeToReady")]
        public static async Task<IActionResult> ChangeToReady(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string userId = req.Query["userId"];
            string url = "callControl/agent/status/ready";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);

            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, "callControl/agent/service/access", accessToken, sessionId);

            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            {
                try
                {
                    string eventUrl = string.Format("{0}://{1}/api/LivePoller_HttpStart",
                     req.Scheme, req.Host);
                    var res = await LivevoxAPI.StartPoller(accessToken, sessionId, eventUrl, userId);
                }
                catch (Exception ex)
                {
                    log.LogInformation("Unable to start the poller");
                }                 
                return (ActionResult)new NoContentResult();
            }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }

        [FunctionName("ManualDialEx")]
        public static async Task<IActionResult> ManualDialEx(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ManualDial function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string phoneNumber = data?.phoneNumber;
            string zipCode = data?.zipCode;
            string accountNumber = data?.accountNumber;

            if (phoneNumber == null)
            {
                return new BadRequestObjectResult("Please specify 'phoneNumber' in the request body");
            }

            ManualDial dial = new ManualDial();
            dial.phoneNumber = phoneNumber;
            dial.accountNumber = accountNumber;
            dial.zipCode = zipCode;

            string json = JsonConvert.SerializeObject(dial);
            //await LivevoxAPI.InvokeLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/customer/call/manual/ex", json, accessToken, sessionId);

            // return (ActionResult)new NoContentResult();

            string url = "callControl/agent/customer/call/manual/ex";
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

        [FunctionName("PreviewDial")]
        public static async Task<IActionResult> PreviewDial(
          [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("PreviewDial function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            PreviewDial dial = new PreviewDial();
            dial.transactionId = data?.transactionId;
            dial.phoneNumber = data?.phoneNumber;

            if (dial.transactionId == null || dial.phoneNumber == null)
            {
                return new BadRequestObjectResult("Please specify 'transactionId' and 'phoneNumber' in the request body");
            }

            string json = JsonConvert.SerializeObject(dial);
            //await LivevoxAPI.InvokeLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/customer/call/preview/dial", json, accessToken, sessionId);

            //return (ActionResult)new NoContentResult();

            string url = "callControl/agent/customer/call/preview/dial";
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

        [FunctionName("PreviewSkip")]
        public static async Task<IActionResult> PreviewSkip(
          [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("PreviewSkip function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string transactionId = req.Query["transactionId"];
            string url = "callControl/agent/customer/call/preview/skip?transactionId=" + transactionId;

            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, url, accessToken, sessionId);
            //return (ActionResult)new NoContentResult();            
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName("Wrap")]
        public static async Task<IActionResult> Wrap(
          [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("Wrap function processed a request.");
            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string url = "callControl/agent/customer/call/end";

            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, url, accessToken, sessionId);

            //return (ActionResult)new NoContentResult();

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName("GetDispositions")]
        public static async Task<IActionResult> GetDispositions(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbGet, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetDispositions function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string serviceId = req.Query["serviceId"];
            string url = "callControl/agent/termCode?serviceId=" + serviceId;

            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, url, accessToken, sessionId);

            //return (serviceId != null)
            //  ? (ActionResult)new OkObjectResult($"{responseText}")
            //  : new BadRequestObjectResult("Please pass serviceId in the request header");

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "GET",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName("SaveDisposition")]
        public static async Task<IActionResult> SaveDisposition(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPut, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("SaveDisposition function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string url = "callControl/agent/call/termCode";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            SaveDisposition dispo = new SaveDisposition();
            dispo.serviceId = data?.serviceId;
            dispo.callTransactionId = data?.callTransactionId;
            dispo.callSessionId = data?.callSessionId;
            dispo.termCodeId = data?.termCodeId;
            dispo.paymentAmt = data?.paymentAmt;
            dispo.account = data?.account;
            dispo.agentEnteredAccount = data?.agentEnteredAccount;
            dispo.phoneDialed = data?.phoneDialed;
            dispo.moveAgentToNotReady = data?.moveAgentToNotReady;
            dispo.reasonCode = data?.reasonCode;
            dispo.immediateCallbackNumber = data?.immediateCallbackNumber;
            dispo.callTransactionId = data?.callTransactionId;

            if (dispo.callTransactionId == null || dispo.callSessionId == null || dispo.termCodeId == null ||
              dispo.phoneDialed == null || dispo.moveAgentToNotReady == null)
            {
                return new BadRequestObjectResult("Please specify 'callTransactionId', 'callSessionId', 'termCodeId', 'phoneDialed' and 'moveToNotReady' in the request body");
            }
            string jsonBody = JsonConvert.SerializeObject(dispo);

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "PUT",
                String.Concat(Dialer.Global.baseUrl, url), jsonBody,
                accessToken, sessionId,
                out jsonResponse);

            //var hostName = req.Headers["HOSTNAME"].ToString();
            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            {
                // Terminte the poller If agnet is not ready for next call
                if (dispo.moveAgentToNotReady == true)
                {
                    // Terminate the poller
                    string eventUrl = string.Format("{0}://{1}/api/LivePollerTerminate?id=",
                                    req.Scheme, req.Host);
                    jsonResponse= jsonResponse + eventUrl;
                    await LivevoxAPI.TerminatePoller(accessToken, sessionId, eventUrl);
                }
                return (ActionResult)new NoContentResult();
            }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }

        [FunctionName("GetScreenPop")]
        public static async Task<IActionResult> GetScreenPop(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbGet, Route = null)]  HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetScreenPop function processed a request.");
            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            string url = "callControl/agent/screenpop";
            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, "callControl/agent/screenpop", accessToken, sessionId);
            // return (ActionResult)new OkObjectResult($"{responseText}");

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "GET",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);

            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, "callControl/agent/service/access", accessToken, sessionId);

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


        [FunctionName("GetListOfServices")]
        public static async Task<IActionResult> GetListOfServices(
           [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbGet, Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            //string serviceId = req.Query["serviceId"];
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            // string sess = data.clientName;
            string url = "callControl/agent/service/access";
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "GET",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);

            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, "callControl/agent/service/access", accessToken, sessionId);

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

        [FunctionName("GetServiceDetails")]
        public static async Task<IActionResult> GetServiceDetails(
            [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbGet, Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string serviceId = req.Query["serviceId"];
            string url = "callControl/agent/service/features?serviceId=" + serviceId;
            //string responseText = await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbGet, url, accessToken, sessionId);

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "GET",
                String.Concat(Dialer.Global.baseUrl, url), null,
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


        [FunctionName("JoinService")]
        public static async Task<IActionResult> JoinService(
           [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            string serviceId = req.Query["serviceId"];
            string phoneNumber = req.Query["phoneNumber"];

            string userId = req.Query["userId"];

            string url = "callControl/agent/service/join?serviceId=" + serviceId + "&phoneNumber=" + phoneNumber;

            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, url, accessToken, sessionId);

            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);
                                   

            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            {
                //start poller
                try
                {
                    string eventUrl = string.Format("{0}://{1}/api/LivePoller_HttpStart",
                     req.Scheme, req.Host);                    
                    var res = await LivevoxAPI.StartPoller(accessToken, sessionId, eventUrl, userId);
                }
                catch (Exception ex)
                {
                    log.LogInformation("Unable to start the poller");
                }
                //return (ActionResult)new OkObjectResult($"{res}");
                return (ActionResult)new NoContentResult();
            }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }

        [FunctionName("LeaveService")]
        public static async Task<IActionResult> LeaveService(
           [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/service/leave", accessToken, sessionId);
            string url = "callControl/agent/service/leave";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
                accessToken, sessionId,
                out jsonResponse);

            if (status == HttpStatusCode.Unauthorized)
            { return (ActionResult)new UnauthorizedObjectResult($"{jsonResponse}"); }
            else if (status == HttpStatusCode.NoContent)
            {
                // Terminate the poller
                string eventUrl = string.Format("{0}://{1}/api/LivePollerTerminate?id=",
                                req.Scheme, req.Host);
                await LivevoxAPI.TerminatePoller(accessToken, sessionId, eventUrl);
                return (ActionResult)new NoContentResult();
            }
            else if (status == HttpStatusCode.BadRequest)
            { return (ActionResult)new BadRequestResult(); }
            else if (status != HttpStatusCode.NoContent)
            { return (ActionResult)new OkObjectResult($"{jsonResponse}"); }
            else { return (ActionResult)new BadRequestResult(); }
        }

        [FunctionName("PauseCallRecording")]
        public static async Task<IActionResult> PauseCallRecording(
         [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
          ILogger log)
        {
            // POST /callControl/agent/callRecord/pause[?lineNumber ={lineNumber}]
            log.LogInformation("PauseCallRecording function processed a request");
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/callRecord/pause", accessToken, sessionId);

            //return (ActionResult)new NoContentResult();

            string url = "callControl/agent/callRecord/pause";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName("ResumeCallRecording")]
        public static async Task<IActionResult> ResumeCallRecording(
        [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
        ILogger log)
        {
            // POST /callControl/agent/callRecord/resume[?lineNumber ={lineNumber}]
            log.LogInformation("ResumeCallRecording function processed a request");
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            //await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/callRecord/resume", accessToken, sessionId);
            //return (ActionResult)new NoContentResult();

            string url = "callControl/agent/callRecord/resume";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName(nameof(HoldCall))]
        public static async Task<IActionResult> HoldCall(
        [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
        ILogger log)
        {
            // POST /callControl/agent/customer/call/hold[?lineNumber={lineNumber}]
            // ACD is default line
            log.LogInformation($"{nameof(HoldCall)} function processed a request");

            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();

            string url = "callControl/agent/customer/call/hold";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName(nameof(RetrieveCall))]
        public static async Task<IActionResult> RetrieveCall(
        [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPost, Route = null)] HttpRequest req,
        ILogger log)
        {
            // POST /callControl/agent/customer/call/unhold[?lineNumber={lineNumber}]
            // ACD is default line
            log.LogInformation($"{nameof(RetrieveCall)} function processed a request");
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();
            // await LivevoxAPI.InvokeNoBodyLivevoxAPI(Dialer.Global.HttpVerbPost, "callControl/agent/customer/call/unhold", accessToken, sessionId);
            //return (ActionResult)new NoContentResult();

            string url = "callControl/agent/customer/call/unhold";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "POST",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName(nameof(Mute))]
        public static async Task<IActionResult> Mute(
        [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPut, Route = null)] HttpRequest req,
        ILogger log)
        {
            // PUT /callControl/agent/call/mute
            log.LogInformation($"{nameof(Mute)} function processed a request");

            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();

            string url = "callControl/agent/call/mute";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "PUT",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

        [FunctionName(nameof(Unmute))]
        public static async Task<IActionResult> Unmute(
        [HttpTrigger(AuthorizationLevel.Anonymous, Dialer.Global.HttpVerbPut, Route = null)] HttpRequest req,
        ILogger log)
        {
            // PUT /callControl/agent/call/unmute
            log.LogInformation($"{nameof(Unmute)} function processed a request");
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            string accessToken = Dialer.Global.Accesstoken;
            string sessionId = req.Headers["LV-Session"].FirstOrDefault();

            string url = "callControl/agent/call/unmute";
            HttpStatusCode status;
            string jsonResponse;

            status = Livevox.MakeRequest(
                "PUT",
                String.Concat(Dialer.Global.baseUrl, url), null,
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

    }
}
