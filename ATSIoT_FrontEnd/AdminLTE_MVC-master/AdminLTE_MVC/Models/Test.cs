using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace AdminLTE_MVC.Models
{
    public class Test
    {

        public List<DataModel> list = new List<DataModel>();

        public void gatherDataFromDate(DateRangeViewModel date1)
        {
            // Create an SqlConnection from the provided connection string.
            string SQLConnectionString = " Server=tcp:astronics.database.windows.net,1433;Database=TestingSQLDatabase;User ID=astronics@astronics;Password=Superluke123;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //List<DataModel> list = new List<DataModel>();

            string start = string.Format("{0}T{1}:01Z",date1.StartDate, date1.StartTime);
            string end = string.Format("{0}T{1}:01Z",date1.EndDate, date1.EndTime);

            using (SqlConnection connection = new SqlConnection(SQLConnectionString))
            {
                // Formulate the command.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Specify the query to be executed.
                command.CommandType = CommandType.Text;

                Console.WriteLine("This is C#");

                // TODO find table name
                command.CommandText = @"
                    SELECT sensorID, timestamp, value, dataType, serverTime
                    FROM dbo.serverTime WHERE timestamp>@starttime AND timestamp<@endtime AND dataType=@datatype;  -- In TestingSQLDatabase database.
                    ";
                command.Parameters.AddWithValue("starttime", start);
                command.Parameters.AddWithValue("endtime", end);
                command.Parameters.AddWithValue("datatype", date1.DataType);


                // Open a connection to database.
                connection.Open();

               

                // Read data returned for the query.
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine("Value:  {0},  {1},  {2},  {3}, {4}", reader[0]
                        , reader[1], reader[2], reader[3], reader[4]);
                    DataModel u = new DataModel();
                    u.sensorID = reader.GetString(0);
                    u.timestamp = reader.GetString(1);
                    u.value = reader.GetString(2).Trim();
                    u.dataType = reader.GetString(3);
                    u.serverTime = reader.GetString(4);
                    u.serverTimeAsDate = DateTime.ParseExact(u.serverTime,"yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AdjustToUniversal);

                    list.Add(u);
                  
                }
                reader.Close();
            }
            Debug.WriteLine(list);
        }


    }
}