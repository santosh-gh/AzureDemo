using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace LivevoxAPI
{
    class LivevoxAPI
    {
        public static async Task<string> InvokeLivevoxAPI(string httpVerb, string extUrl, string json, string accessToken,string sessionId)
        {
            string url = Dialer.Global.baseUrl + extUrl;
            using (var client = new HttpClient())
            {

                var requestData = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                requestData.Headers.Add("LV-Access", accessToken);
                requestData.Headers.Add("LV-Session", sessionId);

                if (httpVerb == Dialer.Global.HttpVerbDelete)
                {
                    var deleteresponse = await client.DeleteAsync(String.Format(url));
                    return await deleteresponse.Content.ReadAsStringAsync();
                }
                else if (httpVerb == Dialer.Global.HttpVerbPut)
                {
                    var putresponse = await client.PostAsync(String.Format(url), requestData);
                    return await putresponse.Content.ReadAsStringAsync();
                }
                else
                {
                    var response = (httpVerb == Dialer.Global.HttpVerbPost) ? await client.PostAsync(String.Format(url), requestData) : await client.GetAsync(String.Format(url));
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        public static async Task<string> InvokeNoBodyLivevoxAPI(string httpVerb, string extUrl,string accessToken, string sessionId)
        {
            string url = Dialer.Global.baseUrl + extUrl;

            var requestData = (HttpWebRequest)WebRequest.Create(url);
            requestData.Headers.Add("LV-Access", accessToken);
            requestData.Headers.Add("LV-Session", sessionId);
            requestData.ProtocolVersion = HttpVersion.Version11;
            requestData.Method = httpVerb;
            requestData.Accept = "gzip,deflate";
            requestData.ContentType = "application/json";

            var httpResponse = (HttpWebResponse)requestData.GetResponse();   
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            //httpResponse.StatusCode
            return streamReader.ReadToEnd(); 
        }

        public static async Task<string> StartPoller(string accessToken, string sessionId, string eventUrl, string userId)
        {
            HttpStatusCode Pollerstatus;
            string jsonRes;

            //string EventUrl = string.Format("{0}://{1}:{2}/api/messages",  
            // req.RequestUri.Scheme, req.RequestUri.Host, req.RequestUri.Port);

            // string url = String.Concat("http://localhost:7071/", "api/LivePoller_HttpStart");
            // string json = "{\"PortalName\":\"NCOTest\",\"SessionId\": \" " + sessionId + "\",\"UserId\":\"123456\" } ";
            string json = String.Format("{{ \"PortalName\" : \"{0}\", \"SessionId\" : \"{1}\", \"UserId\" : \"{2}\" }}", "ncotest", sessionId, userId);
            Pollerstatus = Livevox.MakeRequest("POST", eventUrl, json, accessToken, sessionId, out jsonRes);            
            return jsonRes;
        }
        public static async Task<string> TerminatePoller(string accessToken, string sessionId, string eventUrl)
        {
            HttpStatusCode Pollerstatus;
            string jsonRes = string.Empty;
             
            IDatabase cache = Dialer.RedisStore.Connection.GetDatabase();
            UserSession Existingdata = JsonConvert.DeserializeObject<UserSession>(cache.StringGet(sessionId));
            if (Existingdata != null)
            {
                string url = eventUrl + Existingdata.instanceId;
                Pollerstatus = Livevox.MakeRequest("DELETE", url, null, accessToken, sessionId, out jsonRes);
                 
                // Update Redis Cache
                UserSession session = new UserSession(Existingdata.clientName, Existingdata.userId, Existingdata.sessionId, null);
                cache.StringSet(sessionId, JsonConvert.SerializeObject(session));
            }
            return jsonRes;
        }
    }
}
