using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LivevoxAPI
{
	/// <summary>
	/// Represents the results of an http request to a portal
	/// </summary>
	class LivevoxResponse
	{	
		/// <summary>
		/// Gets the http status code returned from livevox
		/// </summary>
		public HttpStatusCode Status { get; private set; }
		/// <summary>
		/// Gets the json returned in the response body if any
		/// </summary>
		public string Json { get; private set; }

		/// <summary>
		/// Creates a new instance of LivevoxResponse
		/// </summary>
		/// <param name="status">HttpStatusCode returned</param>
		/// <param name="json">Any json from the response body</param>
		public LivevoxResponse(HttpStatusCode status, string json)
		{
			Status = status;
			Json = json;
		}
	}
}
