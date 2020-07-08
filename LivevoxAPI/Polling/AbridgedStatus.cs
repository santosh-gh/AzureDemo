using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI.Polling
{
	/// <summary>
	/// Represents the portion of the AgentStatus that is actually used
	/// </summary>
	class AbridgedStatus
	{
		public readonly static AbridgedStatus Empty;
		static AbridgedStatus()
		{
			Empty = new AbridgedStatus()
			{
				AgentCallConnected = false,
				State = String.Empty,
				ConferenceInitiated = false,
				CallConferenced = false,
				CallTransactionId = String.Empty,
				AgentMuted = false 
			};
		}

		public static AbridgedStatus FromObject(dynamic o)
		{
			if (o == null) return AbridgedStatus.Empty;

			AbridgedStatus status = new AbridgedStatus();

			status.AgentCallConnected = o.agentCallConnected == null ?
				false : (bool)o.agentCallConnected;

			status.AgentMuted = o.agentMuted == null ?
	                      false : (bool)o.agentMuted;

			if (o.lines != null)
			{
				dynamic line = o.lines[0];

				status.State = (string)line.state;

				status.ConferenceInitiated = line.conferenceInitiated == null ?
					false : (bool)line.conferenceInitiated;

				status.CallConferenced = line.callConferenced == null ?
					false : (bool)line.callConferenced;

				if (line.call != null)
				{
					status.CallTransactionId = line.call.callTransactionId == null ?
						String.Empty : (string)line.call.callTransactionId;
				}
				else
				{
					status.CallTransactionId = String.Empty;
				}
			}
			else
			{
				status.State = String.Empty;
				status.ConferenceInitiated = false;
				status.CallConferenced = false;
				status.CallTransactionId = String.Empty;
			}

			return status;
		}

		public static bool NailupChanged(AbridgedStatus s1, AbridgedStatus s2)
		{
			return s1.AgentCallConnected != s2.AgentCallConnected;
		}
		public static bool CallChanged(AbridgedStatus s1, AbridgedStatus s2,
			out bool dialerRunningHot)
		{
			bool change = s1.CallTransactionId != s2.CallTransactionId;
			dialerRunningHot = change &&
				!String.IsNullOrEmpty(s1.CallTransactionId) &&
				!String.IsNullOrEmpty(s2.CallTransactionId);

			return change;
		}

		public static bool WrapChanged(AbridgedStatus s1, AbridgedStatus s2)
		{
			// must be different and one of them must be wrap
			if (s1.State == s2.State) return false;
			return (s1.State == LineState.WRAPUP || s2.State == LineState.WRAPUP);
		}

		 
		public static bool MuteChanged(AbridgedStatus s1, AbridgedStatus s2)
		{ 
			return s1.AgentMuted != s2.AgentMuted;
		}

		public static bool HoldChanged(AbridgedStatus s1, AbridgedStatus s2)
		{
			// must be different and one of them must be wrap
			if (s1.State == s2.State) return false;
			return (s1.State == LineState.HOLD || s2.State == LineState.HOLD);
		}

		public static bool TransferStatusChanged(AbridgedStatus s1, AbridgedStatus s2)
		{
			if (s1.ConferenceInitiated != s2.ConferenceInitiated) return true;
			if (s1.CallConferenced != s2.CallConferenced) return true;
			return false;
		}

		public bool AgentCallConnected { get; private set; }
		public string State { get; private set; }
		public bool ConferenceInitiated { get; private set; }
		public bool CallConferenced { get; private set; }
		public string CallTransactionId { get; private set; }
		public bool AgentMuted { get; private set; }

		//public dynamic GetTransferData()
		//{
		//	dynamic o = new
		//	{
		//		ConferenceInitiated = this.ConferenceInitiated,
		//		CallConferenced = this.CallConferenced
		//	};

		//	return o;
		//}

		public bool IsOnACall
		{ get { return !String.IsNullOrWhiteSpace(this.CallTransactionId); } }
		public bool NailupEstablished
		{ get { return this.AgentCallConnected; } }

		public bool InWrapUp
		{ get { return this.State == LineState.WRAPUP; } }

		public bool NotReady
		{ get { return this.State == LineState.NOTREADY; } }

		public bool IsOnHold
		{ get { return this.State == LineState.HOLD; } }
		 		 
	}
}


