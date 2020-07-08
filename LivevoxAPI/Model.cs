using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI
{
    public class ServiceDetails
    {
        public string serviceId { get; set; }
    }

    public class ManualDial
    {
        public string phoneNumber { get; set; }
        public string accountNumber { get; set; }
        public string zipCode { get; set; }
    }

    public class PreviewDial
    {
        public string transactionId { get; set; }
        public string phoneNumber { get; set; }
    }

    public class SaveDisposition
    {
        public string callTransactionId { get; set; }
        public string callSessionId { get; set; }
        public string termCodeId { get; set; }
        public string phoneDialed { get; set; }
        public bool? moveAgentToNotReady { get; set; }
        public string serviceId { get; set; }
        public string paymentAmt { get; set; }
        public string account { get; set; }
        public string agentEnteredAccount { get; set; }
        public string notes { get; set; }
        public string reasonCode { get; set; }
        public string immediateCallbackNumber { get; set; }
    }

    public class ScreenPopup
    {
        public string Key { get; set; }
        public string Value { get; set; }     
    }


    public class LoginData
    {
        public string clientName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public Boolean agent { get; set; }
    }

    public class ChangePassword
    {
        public string clientName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string newPassword { get; set; }
    }

    public class UserSession
    {
        public string sessionId { get; set; }
        public string userId { get; set; }
        public string clientName { get; set; }
        public string instanceId { get; set; }        

        public UserSession(string clientName, string userId, string sessionId, string instanceId)
        {
            this.clientName = clientName;
            this.userId = userId;
            this.sessionId = sessionId;
            this.instanceId = instanceId;
            //URl, Accesstoken
        }
    }
}
