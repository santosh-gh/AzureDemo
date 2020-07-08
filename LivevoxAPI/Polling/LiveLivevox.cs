using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace LivevoxAPI.Polling
{
	/// <summary>
	/// Class that connects to an actual livevox portal to poll agent status
	/// </summary>
	class LiveLivevox : IPollerConnect
	{
		string _baseUrl;
		string _accessToken;
		string _sessionId;
		ILogger _log;

		public LiveLivevox(string baseUrl, string accessToken,
			string sessionId, ILogger log)
		{
			_baseUrl = baseUrl.EndsWith('/') ?
				baseUrl : String.Concat(baseUrl, '/');

			_accessToken = accessToken;
			_sessionId = sessionId;
			_log = log;
		}

		public void ChangeToReady()
		{
			HttpStatusCode status;
			string json;

			status = Livevox.MakeRequest(
				"POST",
				String.Concat(_baseUrl, "callControl/agent/status/ready"), null,
				_accessToken, _sessionId,
				out json);

			if (status != HttpStatusCode.NoContent)
			{
				_log.LogInformation($"ChangeToReady received {json}");
			}
		}

		public string GetStatus()
		{
			Task<LivevoxResponse> status = Livevox.MakeRequestAsync(
				"POST",
				String.Concat(_baseUrl, "callControl/agent/status"),
				Livevox.EmptyBody,
				_accessToken, _sessionId);

			status.Wait();

			switch (status.Result.Status)
			{
				case HttpStatusCode.OK: return status.Result.Json;

				case HttpStatusCode.Gone: return null;

				default:
					_log.LogInformation($"GetStatus received {status.Result.Json}");

					dynamic error = JsonConvert.DeserializeObject(status.Result.Json);

					// error code 401 means session no longer exists
					if ((int)error.code == 401) return null;
					return String.Empty;
			}
		}

		public void RetrieveCall()
		{
			HttpStatusCode status;
			string json;

			status = Livevox.MakeRequest(
				"POST",
				String.Concat(_baseUrl, "callControl/agent/cusotmer/call/unhold?lineNumber=ACD"),
				null,
				_accessToken, _sessionId,
				out json);

			if (status != HttpStatusCode.NoContent)
			{
				_log.LogInformation($"RetrieveCall received {json}");
			}
		}
	}
}
