using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI.SignalR
{
	public class EventStart
	{
		public string UserId { get; set; }
		public string EventName { get; set; }
		public object EventData { get; set; }
	}

	public class TerminateEventData
	{
        public string Userid { get; set; }

        internal static EventStart Build(string userId)
        {
            return new EventStart()
            {
                UserId = userId,
                EventName = "terminate",
                EventData = new TerminateEventData()
                {
                    Userid = userId
                }
          };
        }
	}

	public class NailupChangedEventData
	{
        public string Userid { get; set; }
        public bool Established { get; set; }

    internal static EventStart Build(string userId, bool established)
    {
      return new EventStart()
      {
        UserId = userId,
        EventName = "nailupChanged",
        EventData = new NailupChangedEventData()
        {
            Userid = userId,
          Established = established
        }
      };
    }
	}

	public class CallArrivedEventData
	{
    public string Userid { get; set; }
    public string State { get; set; }
    public int CallsRemaining { get; set; }
    public string CallTransactionId { get; set; }
    public string CallSessionId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool CallRequiresAcceptance { get; set; }
    public int CallAcceptanceTimeout { get; set; }
    public bool CallRecordingStarted { get; set; }
    public int ServiceId { get; set; }
    public int CallCenterId { get; set; }
    public string AccountNumber { get; set; }
    public string PhoneNumber { get; set; }
    public bool AccountNumberRequired { get; set; }

    internal static EventStart Build(string userId, dynamic status)
    {
      return new EventStart()
      {
        UserId = userId,
        EventName = "callArrived",
        EventData = FromObject(status)
      };
    }
    private static CallArrivedEventData FromObject(dynamic o)
    {
      dynamic line = o.lines[0];
      dynamic call = line.call;

      CallArrivedEventData data = new CallArrivedEventData();

      data.State = (string)line.state;
      data.CallsRemaining = (int)o.service.callsRemaining;
      data.CallTransactionId = (string)call.callTransactionId;
      data.CallSessionId = (string)call.callSessionId;
      data.FirstName = (string)call.firstName;
      data.LastName = (string)call.lastName;

      data.CallRequiresAcceptance = call.callRequiresAcceptance == null ?
        false : (bool)call.callRequiresAcceptance;

      data.CallAcceptanceTimeout = call.callAcceptanceTimeout == null ?
        0 : (int)call.callAcceptanceTimeout;

      data.CallRecordingStarted = (bool)call.callRecordingStarted;
      data.ServiceId = (int)call.serviceId;
      data.CallCenterId = (int)call.callCenterId;
      data.AccountNumber = (string)call.accountNumber;
      data.PhoneNumber = (string)call.phoneNumber;
      data.AccountNumberRequired = (bool)call.accountNumberRequired;

      return data;
    }
  }

  public class CallEndedEventData
  {
     public string Userid { get; set; }
     public bool NotReady { get; set; }

    internal static EventStart Build(string userId, bool notReady)
    {
      return new EventStart()
      {
        UserId = userId,
        EventName = "callEnded",
        EventData = new CallEndedEventData()
        {
            Userid = userId,
            NotReady = notReady
        }
      };
    }
  }

  public class WrapChangedEventData
  {
    public string Userid { get; set; }
    public bool Wraped { get; set; }

    internal static EventStart Build(string userId, bool wraped)
    {
      return new EventStart()
      {
        UserId = userId,
        EventName = "wrapChanged",
        EventData = new WrapChangedEventData()
        {
          Userid = userId,
          Wraped = wraped
        }
      };
    }
  }
        
    public class MuteChangedEventData
    {
        public string Userid { get; set; }
        public bool Muted { get; set; }

        internal static EventStart Build(string userId, bool Muted)
        {
            return new EventStart()
            {
                UserId = userId,
                EventName = "MuteChanged",
                EventData = new MuteChangedEventData()
                {
                    Userid = userId,
                    Muted = Muted
                }
            };
        }
    }

    public class HoldChangedEventData
    {
        public string Userid { get; set; }
        public bool Hold { get; set; }

        internal static EventStart Build(string userId, bool hold)
        {
            return new EventStart()
            {
                UserId = userId,
                EventName = "HoldChanged",
                EventData = new HoldChangedEventData()
                {
                    Userid = userId,
                    Hold = hold
                }
            };
        }
    }

    public class TranserStatusChangedEventData
  {
    public string Userid { get; set; }
    public bool ConferenceInitiated { get; set; }
    public bool CallConferenced { get; set; }

    internal static EventStart Build(string userId, Polling.AbridgedStatus status)
    {
      return new EventStart()
      {
        UserId = userId,
        EventName = "transferStatusChanged",
        EventData = new TranserStatusChangedEventData()
        {
          Userid = userId,
          ConferenceInitiated = status.ConferenceInitiated,
          CallConferenced = status.CallConferenced
        }
      };
    }
  }
}

