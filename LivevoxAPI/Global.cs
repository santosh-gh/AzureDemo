using System;
using StackExchange.Redis;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Dialer
{

    public class Global
    {
        public const String baseUrl = "https://api.livevox.com/";
        public const String Accesstoken = "0fdd7dd0-81ec-45ba-9d6a-47ca398d6805";

        public const String HttpVerbGet = "get";
        public const String HttpVerbPost = "post";
        public const String HttpVerbPut = "put";
        public const String HttpVerbDelete = "delete";
        public const String HttpVerbPatch = "patch";
        public const String AzureWebJobsStorage = "AzureWebJobsStorage";
        public const String MessageTarget = "notify";
        public const String SignalRHubName = "notifications";
    }
    public class RedisStore
    {
        //public static IDatabase cache = null;
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            //string cacheConnection = ConfigurationManager.AppSettings["RedisConnectionString"].ToString();
            //  string cacheConnection = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;

            var cacheConnection = "livevox-redis.redis.cache.windows.net:6379,password=QXfVfokhTLBHEqg0pi5XuW2ixbmFm54TUlGS2ERVUs4=,ssl=False,abortConnect=False,connectTimeout=1000";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        public void Dispose()
        {
            if (lazyConnection != null && lazyConnection.IsValueCreated)
            {
                lazyConnection.Value.Dispose();
            }
        }
    }
    class PortalInfo : TableEntity
    {
        private string portalId;
        private string url;
        private string accessToken;

        public void AssignRowKey()
        {
            this.RowKey = portalId.ToString();
        }
        public void AssignPartitionKey()
        {
            this.PartitionKey = accessToken;
        }
        public string PortalID
        {
            get
            {
                return portalId;
            }

            set
            {
                portalId = value;
            }
        }
        public string URL
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }
        public string AccessToken
        {
            get
            {
                return accessToken;
            }

            set
            {
                accessToken = value;
            }
        }

        public static async void CreateNewTable(CloudTable table)
        {
            if (!await table.CreateIfNotExistsAsync())
            {
                return;
            }
        }
        public static void InsertRecordToTable(CloudTable table)
        {

            PortalInfo customerEntity = new PortalInfo();
            customerEntity.PortalID = "PortalID";
            customerEntity.URL = "livevox.com";
            customerEntity.AccessToken = "AccessToken";
            customerEntity.AssignPartitionKey();
            customerEntity.AssignRowKey();
            //Customer custEntity = RetrieveRecord(table, customerType, customerID);
            //if (custEntity == null)
            {
                TableOperation tableOperation = TableOperation.Insert(customerEntity);
                table.ExecuteAsync(tableOperation);
            }
        }
    }
    class SignalRConnInfo : TableEntity
    {

        private string userId;
        private string connectionId;

        public void AssignRowKey()
        {
            this.RowKey = userId.ToString();
        }
        public void AssignPartitionKey()
        {
            this.PartitionKey = "100";
        }
        public string ConnectionID
        {
            get
            {
                return connectionId;
            }

            set
            {
                connectionId = value;
            }
        }
        public string UserID
        {
            get
            {
                return userId;
            }

            set
            {
                userId = value;
            }
        }
        public static async void CreateNewTable(CloudTable table)
        {
            if (!await table.CreateIfNotExistsAsync())
            {
                return;
            }
        }
        public static void InsertRecordToTable(CloudTable table, string userid, string connectionid)
        {
            SignalRConnInfo connInfo = new SignalRConnInfo();
            connInfo.UserID = userid;
            connInfo.ConnectionID = connectionid;
            connInfo.AssignPartitionKey();
            connInfo.AssignRowKey();
           
            TableOperation tableOperation = TableOperation.Insert(connInfo);
            table.ExecuteAsync(tableOperation);
        }
        public static async Task<SignalRConnInfo> RetrieveRecord(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve<SignalRConnInfo>(partitionKey, rowKey);
            TableResult tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result as SignalRConnInfo;
        }

        public static void UpdateRecordInTable(CloudTable table, string userid, string connectionid)
        {
            Task<Dialer.SignalRConnInfo> task = Task.Run<Dialer.SignalRConnInfo>(async () => await RetrieveRecord(table, "100", userid));

            SignalRConnInfo sci = task.Result;
            if (sci != null)
            {
                sci.UserID = userid;
                sci.connectionId = connectionid;
                TableOperation tableOperation = TableOperation.Replace(sci);
                table.ExecuteAsync(tableOperation);              
            }            
        }
    }
}
