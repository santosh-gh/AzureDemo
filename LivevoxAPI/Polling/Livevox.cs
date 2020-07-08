using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LivevoxAPI
{
	/// <summary>
	/// Provides methods for communicating with a livevox portal
	/// </summary>
	static class Livevox
	{
		/// <summary>
		/// The maximum number of attampts a request will be made if
		/// there is no response from livevox
		/// </summary>
		const int MAX_RETRIES = 60;

		/// <summary>
		/// Empty body for calls that have request bodies consisting of
		/// all optional values, but the existance of the body itself is
		/// not optional
		/// </summary>
		public static readonly string EmptyBody = "{ }";

		/// <summary>
		/// Makes a request to the livevox api
		/// </summary>
		/// <param name="method">Http method used, ie: GET, POST</param>
		/// <param name="url">Complete url of the livevox portal</param>
		/// <param name="body">A json serializable object to be sent in the request body (optional)</param>
		/// <param name="accessToken">The access token for the portal</param>
		/// <param name="sessionId">The agent's session id received after login</param>
		/// <param name="json">Output parameter returning anj resulting json from the response</param>
		/// <returns>Returns the http status code</returns>
		public static HttpStatusCode MakeRequest(
			string method, string url,
			object body, string accessToken, string sessionId,
			out string json)
		{
			int triesLeft = MAX_RETRIES;
			json = String.Empty;

			do
			{
				HttpWebRequest req = CreateRequest(
					method, url, body, accessToken, sessionId);

				try
				{
					HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

					if (rsp.StatusCode == HttpStatusCode.OK)
					{
						json = ReadStream(rsp.GetResponseStream());
					}

					return rsp.StatusCode;
				}
				catch (WebException ex)
				{
					if (ex.Response == null)
					{
						System.Threading.Thread.Sleep(1000);
						continue;
					}
					else
					{
						HttpWebResponse rsp = (HttpWebResponse)ex.Response;
						json = ReadStream(rsp.GetResponseStream());
						return rsp.StatusCode;
					}
				}

			} while (--triesLeft > 0);

			json = null;
			return HttpStatusCode.Gone;
		}

		/// <summary>
		/// Makes an asynchronous request to the livevox api
		/// </summary>
		/// <param name="method">Http method used, ie: GET, POST</param>
		/// <param name="url">Complete url of the livevox portal</param>
		/// <param name="body">A json serializable object to be sent in the request body (optional)</param>
		/// <param name="accessToken">The access token for the portal</param>
		/// <param name="sessionId">The agent's session id received after login</param>
		/// <returns>Returns a LivevoxResponse object</returns>
		public static async Task<LivevoxResponse> MakeRequestAsync(
			string method, string url,
			object body, string accessToken, string sessionId)
		{
			int triesLeft = MAX_RETRIES;

			do
			{
				HttpWebRequest req = CreateRequest(
					method, url, body, accessToken, sessionId);

				try
				{
					HttpWebResponse rsp = (HttpWebResponse)await req.GetResponseAsync();

					return new LivevoxResponse(
						rsp.StatusCode,
						rsp.StatusCode == HttpStatusCode.OK ?
							ReadStream(rsp.GetResponseStream()) : String.Empty);
				}
				catch (WebException ex)
				{
					if (ex.Response == null)					
					{
						System.Threading.Thread.Sleep(1000);
						continue;
					}
					else
					{
						HttpWebResponse rsp = (HttpWebResponse)ex.Response;

						return new LivevoxResponse(
							rsp.StatusCode,
							ReadStream(rsp.GetResponseStream()));
					}
				}

			} while (--triesLeft > 0);

			return new LivevoxResponse(HttpStatusCode.Gone, null);
		}

		/// <summary>
		/// Boilerplate code to stamp out a livevox http request
		/// </summary>
		/// <param name="method">Http method used, ie: GET POST</param>
		/// <param name="url">The complete url of the request</param>
		/// <param name="body">Optional json serializable request body</param>
		/// <param name="accessToken">The portal's access token</param>
		/// <param name="sessionId">The agents session id</param>
		/// <returns></returns>
		private static HttpWebRequest CreateRequest(
			string method, string url,
			object body, string accessToken, string sessionId)
		{
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

			req.Method = method;
			req.ProtocolVersion = HttpVersion.Version11;
			req.Accept = "gzip,deflate";
			req.ContentType = "application/json";
			req.Headers.Add("LV-Access", accessToken);

			if (!String.IsNullOrEmpty(sessionId))
			{
				req.Headers.Add("LV-Session", sessionId);
			}

			if (body != null)
			{
				using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
				{
					sw.Write(body is string ? body : JsonConvert.SerializeObject(body));
				}
			}

			return req;
		}

		/// <summary>
		/// Helper function to read an entire stream
		/// </summary>
		/// <param name="s">The stream to read</param>
		/// <returns>Returns the entire contents of the stream</returns>
		private static string ReadStream(Stream s)
		{
			using (StreamReader sr = new StreamReader(s))
			{
				return sr.ReadToEnd();
			}
		}
	}
}
