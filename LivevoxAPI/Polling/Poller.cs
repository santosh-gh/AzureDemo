using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI.Polling
{
	/// <summary>
	/// This class implements the actual poller logic
	/// </summary>
	static class Poller
	{
		/// <summary>
		/// Max number of errors allowed before agent establishes nailup for the first time.
		/// (Until they are in the system, a request for the status fails)
		/// </summary>
		const int MAX_PRENAILUP_ERRORS = 300;

		/// <summary>
		/// Maximum time the poller will run
		/// </summary>
		const int MAX_RUNTIME_HOURS = 8;

		/// <summary>
		/// The poller function
		/// </summary>
		/// <param name="livevox">Object that retrieves agent statuses</param>
		/// <param name="userId">SignalR connection id for sending events.</param>
		public static void Run(IPollerConnect livevox, string eventUrl, string userId, ILogger log)
		{
			int preNailupErrorsRemaining = MAX_PRENAILUP_ERRORS;
			DateTime timeout = DateTime.Now.AddHours(MAX_RUNTIME_HOURS);
			AbridgedStatus lastStatus = AbridgedStatus.Empty;

			while (DateTime.Now < timeout)
			{
				System.Threading.Thread.Sleep(1000);

				string json = livevox.GetStatus();

				if (!String.IsNullOrEmpty(json))
				{
					preNailupErrorsRemaining = 0;

					dynamic fullStatus = ExtractStatus(json);

					AbridgedStatus nextStatus = AbridgedStatus.FromObject(fullStatus);
					


					DispatchEvents(
						livevox, eventUrl, userId, fullStatus, lastStatus, nextStatus, log);
					
					lastStatus = nextStatus;
				}
				else if (json == String.Empty)
				{
					// agent not yet in the system
					if (--preNailupErrorsRemaining <= 0) break;
					continue;
				}
				else
				{
					// null = session gone or network failed, abort
					break;
				}
			}
			RaiseTerminateEvent(eventUrl, userId, log);
		}

		private static void DispatchEvents(IPollerConnect livevox, 
			string eventUrl, string userId,	dynamic fullStatus,
			AbridgedStatus lastStatus, AbridgedStatus nextStatus, ILogger log)
		{
			if (AbridgedStatus.NailupChanged(lastStatus, nextStatus))
			{
				if (nextStatus.NailupEstablished)
				{
					if (nextStatus.NotReady)
					{
 
            livevox.ChangeToReady();
 
					}
					else if (nextStatus.IsOnHold)
					{
						livevox.RetrieveCall();
					}
				}

				RaiseNailupEvent(eventUrl, userId, 
					nextStatus.NailupEstablished, log);
			}

			bool hot;
			if (AbridgedStatus.CallChanged(lastStatus, nextStatus, out hot))
			{
				if (nextStatus.IsOnACall)
				{
					if (hot) RaiseCallEndedEvent(eventUrl, userId, false, log);
					RaiseCallArrivedEvent(eventUrl, userId, fullStatus, log);
				}
				else
				{
					RaiseCallEndedEvent(eventUrl, userId, nextStatus.NotReady, log);
				}
			}
			else if (nextStatus.IsOnACall)
			{
				if (AbridgedStatus.TransferStatusChanged(lastStatus, nextStatus))
				{
					RaiseTransferStatusChangedEvent(eventUrl, userId, nextStatus, log);
				}

				if (AbridgedStatus.WrapChanged(lastStatus, nextStatus))
				{
					RaiseWrapChangedEvent(eventUrl, userId, nextStatus.InWrapUp, log);
				}
				 
				if (AbridgedStatus.MuteChanged(lastStatus, nextStatus))
				{
					RaiseMuteChangedEvent(eventUrl, userId, nextStatus.AgentMuted, log);
				}

				if (AbridgedStatus.HoldChanged(lastStatus, nextStatus))
				{
					RaiseHoldChangedEvent(eventUrl, userId, nextStatus.IsOnHold, log);
				}
			}	
 
		}

		static dynamic ExtractStatus(string json)
		{
			dynamic statusArray = JsonConvert.DeserializeObject(json);
			return statusArray.agentStatus[0];
		}

		private static void RaiseTerminateEvent(string url, string userId, ILogger log)
		{
			log.LogInformation(">>>>> Raise Terminate Event");
			SignalR.SignalREvents.SendMessage(
				url, 
				SignalR.TerminateEventData.Build(userId));
		}

		private static void RaiseNailupEvent(string url, string userId, bool established, ILogger log)
		{
			log.LogInformation($">>>>> Raise Nailup Event, Established = {established}");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.NailupChangedEventData.Build(userId, established));
		}

		private static void RaiseCallArrivedEvent(string url, string userId, dynamic status, ILogger log)
		{
			log.LogInformation(">>>>> Raise Call Arrived Event");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.CallArrivedEventData.Build(userId, status));
		}
		private static void RaiseCallEndedEvent(string url, string userId, bool notReady, ILogger log)
		{
			log.LogInformation($">>>>> Raise Call Ended Event, notReady = {notReady}");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.CallEndedEventData.Build(userId, notReady));
		}
		private static void RaiseWrapChangedEvent(string url, string userId, bool wraped, ILogger log)
		{
			log.LogInformation($">>>>> Raise Wrap Changed Event, wraped = {wraped}");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.WrapChangedEventData.Build(userId, wraped));
		}
				 
		private static void RaiseMuteChangedEvent(string url, string userId, bool Muted, ILogger log)
		{
			log.LogInformation($">>>>> Raise CallMuted Changed Event, Muted = {Muted}");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.MuteChangedEventData.Build(userId, Muted));
		}

		private static void RaiseHoldChangedEvent(string url, string userId, bool Hold, ILogger log)
		{
			log.LogInformation($">>>>> Raise Hold Changed Event, Hold = {Hold}");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.HoldChangedEventData.Build(userId, Hold));
		}

		private static void RaiseTransferStatusChangedEvent(string url, string userId, AbridgedStatus status, ILogger log)
		{
			log.LogInformation(">>>>> Raise Transfer Changed Event");

			SignalR.SignalREvents.SendMessage(
				url,
				SignalR.TranserStatusChangedEventData.Build(userId, status));
		}
	}
}
