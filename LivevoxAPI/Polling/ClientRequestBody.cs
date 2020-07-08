using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI
{
	/// <summary>
	/// This class or a class inherriting from it should be passed with
	/// all client http requests
	/// </summary>
	public class ClientRequestBody
	{
		/// <summary>
		/// Gets or sets the name of portal request is addressed to
		/// </summary>
		public string PortalName { get; set; }

		/// <summary>
		/// Gets or sets the id of agent session.  May be left blank on agent login
		/// </summary>
		public string SessionId { get; set; }
	}
}
