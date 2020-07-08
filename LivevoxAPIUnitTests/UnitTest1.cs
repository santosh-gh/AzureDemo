using System;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.IO;

namespace LivevoxAPIUnitTests
{
    public class UnitTest1        
    {
        string baseUrl = "https://apim-dialer.azure-api.net/devlivevoxapi/";
       // string baseUrl = "http://localhost:7071/api/";

        [Fact]
        public async void TestLoginSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"Login");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";
             
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))   
            {
                string json = "{\"clientName\":\"NCOTEst\",\"userName\":\"SIREESHA_AGENT\",\"password\":\"livevox2019\",\"agent\":true} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();
             
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;     

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestLogoutSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"Logout?sessionId=a59020e8-b121-45dc-87b6-5f43d56c6b3d");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "DELETE";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestChangePasswordSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"ChangePassword");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"clientName\":\"NCOTEst\",\"userName\":\"SIREESHA_AGENT\",\"password\":\"testpsw\",\"newPassword\":newtestpsw} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var responseText = streamReader.ReadToEnd();                     
            //}

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestGetListOfServicesSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"GetListOfServices");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
           // request.Headers.Add("LV-Session", "123456789");
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;                                       
            
            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestGetServiceDetailsSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"GetServiceDetails?serviceId=12121");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestGetDispositionsSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"GetDispositions?serviceId=12121");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestGetScreenPopSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"GetScreenPop");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "GET";

            var httpResponse = await request.GetResponseAsync();
            //var responseText = string.Empty;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the response is an "OK" response
            // Assert.IsAssignableFrom<OkObjectResult>(httpResponse);

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status200OK, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestJoinServiceSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"JoinService?serviceId=12121");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents              
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestLeaveServiceSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"LeaveService?serviceId=12121");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents             
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestChangeToReadySuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"ChangeToReady");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            var httpResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents             
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestChangeToNotReadySuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"ChangeToNotReady");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"reasonCode\":\"Test\"} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();         

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestManualDialSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"ManualDial");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"phoneNumber\":\"2625571234\",\"zipCode\":\"12345\",\"accountNumber\":\"123456789\"} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestPreviewDialSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"PreviewDial");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"transactionId\":\"123456789\",\"phoneNumber\":\"2625571234\"} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestPreviewSkipSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"PreviewSkip");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"transactionId\":\"123456789\"} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestWrapSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"Wrap");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

             
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

        [Fact]
        public async void TestSaveDispositionSuccess()
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) != System.Net.SecurityProtocolType.Tls12)
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }

            var request = (HttpWebRequest)WebRequest.Create(baseUrl+"SaveDisposition");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "8b144861420d4c9f8d7fa40704346f83");
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"callTransactionId\":\"123456789\",\"callSessionId\":\"123456123456\",\"termCodeId\":\"123456123456\",\"paymentAmount\":\"123456123456\"" +
                    ",\"accountNumber\":\"123456123456\",\"phoneDialed\":\"123456123456\",\"moveToNotReady\":\"true\",\"notReadyReason\":\"123456123456\"} ";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = await request.GetResponseAsync();

            var response = (HttpWebResponse)httpResponse;
            var statusNumber = (int)response.StatusCode;
            var responseText = (HttpWebResponse)httpResponse;

            // Check that the contents of the response are the expected contents            
            Assert.Equal(StatusCodes.Status204NoContent, (int)responseText.StatusCode);
        }

    }
}
