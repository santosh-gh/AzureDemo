using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI.Polling
{
	static class LineState
	{
		public static readonly string NOTREADY = "NOTREADY";
		public static readonly string HOLD = "HOLD"; 
		public static readonly string AgentMuted = "AgentMuted";		 
		public static readonly string WRAPUP = "WRAPUP";
		public static readonly string PREVIEWDIAL = "PREVIEWDIAL";
	}
}
