using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI
{
	/// <summary>
	/// Contains information about a portal
	/// </summary>
	public class PortalInfo
	{
		/// <summary>
		/// Gets or sets the base url of the portal
		/// </summary>
		public string BaseUrl { get; set; }

		/// <summary>
		/// Gets or sets the access token required to commumicate with the portal
		/// </summary>
		public string AccessToken { get; set; }
	}
}
