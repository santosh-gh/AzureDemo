//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AzureLivevoxClient.Polling
//{
//  class CallData
//  {
//    public string State { get; set; }
//    public int CallsRemaining { get; set; }
//    public string CallTransactionId { get; set; }
//    public string CallSessionId { get; set; }
//    public string FirstName { get; set; }
//    public string LastName { get; set; }
//    public bool CallRequiresAcceptance { get; set; }
//    public int CallAcceptanceTimeout { get; set; }
//    public bool CallRecordingStarted { get; set; }
//    public int ServiceId { get; set; }
//    public int CallCenterId { get; set; }
//    public string AccountNumber { get; set; }
//    public string PhoneNumber { get; set; }
//    public bool AccountNumberRequired { get; set; }

//    public static CallData FromObject(dynamic o)
//    {
//      dynamic line = o.lines[0];
//      dynamic call = line.call;

//      CallData data = new CallData();

//      data.State = (string)line.state;
//      data.CallsRemaining = (int)o.service.callsRemaining;
//      data.CallTransactionId = (string)call.callTransactionId;
//      data.CallSessionId = (string)call.callSessionId;
//      data.FirstName = (string)call.firstName;
//      data.LastName = (string)call.lastName;

//      data.CallRequiresAcceptance = call.callRequiresAcceptance == null ?
//        false : (bool)call.callRequiresAcceptance;

//      data.CallAcceptanceTimeout = call.callAcceptanceTimeout == null ?
//        0 : (int)call.callAcceptanceTimeout;

//      data.CallRecordingStarted = (bool)call.callRecordingStarted;
//      data.ServiceId = (int)call.serviceId;
//      data.CallCenterId = (int)call.callCenterId;
//      data.AccountNumber = (string)call.accountNumber;
//      data.PhoneNumber = (string)call.phoneNumber;
//      data.AccountNumberRequired = (bool)call.accountNumberRequired;

//      return data;
//    }
//  }
//}
