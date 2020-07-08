using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI
{
	/// <summary>
	/// Provides methods for looking up information about portals
	/// </summary>
	static class Portal
	{
		static PortalInfo s_testPortal = new PortalInfo()
		{
			BaseUrl = "https://api.livevox.com",
			AccessToken = "0fdd7dd0-81ec-45ba-9d6a-47ca398d6805"
		};
		public static PortalInfo GetPortalInfo(string portalName)
		{
			return s_testPortal;
		}
	}
}
