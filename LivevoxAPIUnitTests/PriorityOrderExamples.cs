using System.Collections.Generic;
using System;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Collections.Concurrent;
using Xunit.Sdk;
using System.Linq;
using System.Reflection;
using LivevoxAPIUnitTests;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace LivevoxAPIUnitTests
{
    //[TestCaseOrderer("FullNameOfOrderStrategyHere", "OrderStrategyAssemblyName")]

    public class LoginResult
    {
        public string sessionId { get; set; }
        public string userId { get; set; }
        public string clientId { get; set; }        
    }


    [TestCaseOrderer("LivevoxAPIUnitTests.PriorityOrderer", "LivevoxAPIUnitTests")]
    public class PriorityOrderExamples
    {
        string baseUrl = "https://apim-dialer.azure-api.net/livevoxfunctionapp/";
        string SessionId = "96d18a50-a08a-4019-bd19-30aabfd99b35";
        string Accesstoken = "0fdd7dd0-81ec-45ba-9d6a-47ca398d6805";
        [Fact, TestPriority(0)]
        public async void TestLoginSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "Login");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            //request.Headers.Add("LV-Session", SessionId);
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"clientName\":\"NCOTEST\",\"userName\":\"SIREESHA_AGENT\",\"password\":\"livevox2020\",\"agent\":true} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            string responseText = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }

            var dataObjects = JsonConvert.DeserializeObject<LoginResult>(responseText.ToString());
            SessionId= dataObjects.sessionId;
            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact, TestPriority(1)]
        public async void TestGetListOfServicesSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "GetListOfServices");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            request.Headers.Add("LV-Session", SessionId);
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact, TestPriority(2)]
        public async void TestJoinServiceSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "JoinService?serviceId=66171&phoneNumber=1234");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            request.Headers.Add("LV-Session", SessionId);
            request.Method = "POST";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents              
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact, TestPriority(3)]
        public async void TestGetServiceDetailsSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "GetServiceDetails?serviceId=66171");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            request.Headers.Add("LV-Session", SessionId);
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact, TestPriority(4)]
        public async void TestGetDispositionsSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "GetDispositions?serviceId=66171");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            request.Headers.Add("LV-Session", SessionId);
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }


        [Fact, TestPriority(5)]
        public async void TestLeaveServiceSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "LeaveService?serviceId=66171");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Headers.Add("LV-Access", Accesstoken);
            request.Headers.Add("LV-Session", SessionId);
            request.Method = "POST";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents             
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }
    }
}
