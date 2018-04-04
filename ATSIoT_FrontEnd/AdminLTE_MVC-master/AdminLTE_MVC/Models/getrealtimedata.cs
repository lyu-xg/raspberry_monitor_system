using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace AdminLTE_MVC.Models
{
    public class getrealtimedata
    {
        
        string SQLConnectionString = " Server=tcp:astronics.database.windows.net,1433;Database=TestingSQLDatabase;User ID=astronics@astronics;Password=Superluke123;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        static string iotHubconnectionString = "HostName=UCI.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=Gc0/SjDzb9xPVMIBi4wJBso29wOrvijkR97ggmGI/aU=";
        static EventHubClient eventHubClient;
        public List<DataModel> temperatureList = new List<DataModel>();
        public List<DataModel> accelerationList = new List<DataModel>();

        // original method.
        public List<DataModel> gatherrealtimedata1(string datatype)
        {
            
            // Create an SqlConnection from the provided connection string.
            List<DataModel> list = new List<DataModel>();

            using (SqlConnection connection = new SqlConnection(SQLConnectionString))
            {
                // Formulate the command.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Specify the query to be executed.
                command.CommandType = CommandType.Text;
                // TODO find table name
                command.CommandText = @"
                    SELECT TOP 1 sensorID, timestamp, value, dataType, serverTime
                    FROM dbo.serverTime WHERE dataType=@datatype
                    ORDER BY serverTime DESC;  -- In TestingSQLDatabase database.
                    ";
                
                command.Parameters.AddWithValue("datatype", datatype);
                // Open a connection to database.
                connection.Open();

                // Read data returned for the query.
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine("Values:  {0}, {1}, {2}, {3}, {4}",
                        reader[0], reader[1], reader[2], reader[3], reader[4]);
                    DataModel u = new DataModel();
                    u.sensorID = reader.GetString(0);
                    u.timestamp = reader.GetString(1);
                    u.value = reader.GetString(2).Trim();
                    u.dataType = reader.GetString(3);
                    u.serverTime = reader.GetString(4);

                    list.Add(u);
                }
                reader.Close();
            }
            
            return list;
        }

        public async Task gatherrealtimedata()
        {
            List<DataModel> list = new List<DataModel>();
            eventHubClient = EventHubClient.CreateFromConnectionString(iotHubconnectionString, "messages/events");
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            Debug.WriteLine("entering listening loop in background");
            foreach (string partition in d2cPartitions)
            {
                var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
                while (true)
                {
                    EventData eventData = await eventHubReceiver.ReceiveAsync();
                    if (eventData == null) continue;

                    string data = Encoding.UTF8.GetString(eventData.GetBytes());
                    dynamic reader = JsonConvert.DeserializeObject(data);
                    DataModel u = new DataModel();
                    u.sensorID = reader.sensorID;
                    u.timestamp = reader.timestamp;
                    u.value = reader.value.Trim();
                    u.dataType = reader.dataType;
                    u.serverTime = reader.serverTime;

                    if (u.dataType=="te") temperatureList.Add(u);
                    if (u.dataType == "ac") accelerationList.Add(u);
                    Debug.WriteLine(string.Format("Message received. Data: '{0}'", data));
                }
            }
            Debug.WriteLine("leaving loop"); 

        }
        public List<DataModel>  getrealtimetemperature()
        {
            return temperatureList;
        }
        public List<DataModel> getrealtimeacceleration()
        {
            return accelerationList;
        }

    }
}