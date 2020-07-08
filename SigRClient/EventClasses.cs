using System;
using System.Collections.Generic;
using System.Text;

namespace SigRClient
{
  public class TerminateEventData
  {
        public string Userid { get; set; }
  }

  public class NailupChangedEventData
  {
    public bool Established { get; set; }
  }

  public class CallArrivedEventData
  {
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
  }

  public class CallEndedEventData
  {
    public bool NotReady { get; set; }
  }

  public class WrapChangedEventData
  {
    public bool Wraped { get; set; }
  }

  public class TranserStatusChangedEventData
  {
    public bool ConferenceInitiated { get; set; }
    public bool CallConferenced { get; set; }
  }
}
