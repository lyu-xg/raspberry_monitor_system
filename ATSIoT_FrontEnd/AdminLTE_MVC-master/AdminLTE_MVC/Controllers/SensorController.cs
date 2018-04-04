using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using AdminLTE_MVC.Models;
using System.IO;
using System.Threading.Tasks;

namespace AdminLTE_MVC.Controllers
{
    public class SensorController : Controller
    {

        // temperature real time
        public getrealtimedata realtime_data_base = new getrealtimedata();
        public ActionResult TemperatureRealTime(string datatype)
        {
            Task realtime_data_Task = Task.Run(() => realtime_data_base.gatherrealtimedata());
            return View();
        }


        // temperature history

        public ActionResult TemperatureHistory()
        {
            Models.Test data_base = new Models.Test();
            return View(data_base);
        }

        [HttpPost]
        [ActionName("TemperatureHistory")]
        public ActionResult getTemperatureDate(string daterange, string Download, string datatype)
        {
            string[] dates = daterange.Split();

            //input date range to model
            Models.DateRangeViewModel model = new Models.DateRangeViewModel();
            model.StartDate = dates[0];
            if ((Convert.ToInt32(dates[1].Substring(0, 1)) < 10) && (dates[1].Substring(1, 2) == ":"))
            {

                model.StartTime = "0" + dates[1];

            }
            else
            {
                model.StartTime = dates[1];
            }
            model.EndDate = dates[3];
            if ((Convert.ToInt32(dates[4].Substring(0, 1)) < 10) && dates[4].Substring(1, 2) == ":")
            {
                model.EndTime = "0" + dates[4];

            }
            else
            {
                model.EndTime = dates[4];
            }
            model.DataType = datatype;


            // build model for connecting database
            Models.Test data_base = new Models.Test();
            data_base.gatherDataFromDate(model);


            if (Download != null)
            {
                ExportClientsListToCSV(data_base.list);
            }
            else
            {
                Debug.WriteLine(model.StartDate);
                Debug.WriteLine(model.StartTime);

            }



            return View(data_base);

        }


        // acceleration real time
        public ActionResult AccelerationRealTime(string datatype)
        {

            return View();
        }


        //acceleration history

        public ActionResult AccelerationHistory()
        {
            Models.Test data_base = new Models.Test();
            return View(data_base);
        }

        [HttpPost]
        [ActionName("AccelerationHistory")]
        public ActionResult getAccelerationDate(string daterange, string Download, string datatype)
        {
            string[] dates = daterange.Split();

            //input date range to model
            Models.DateRangeViewModel model = new Models.DateRangeViewModel();
            model.StartDate = dates[0];
            if ((Convert.ToInt32(dates[1].Substring(0, 1)) < 10) && (dates[1].Substring(1, 2) == ":"))
            {

                model.StartTime = "0" + dates[1];

            }
            else
            {
                model.StartTime = dates[1];
            }
            model.EndDate = dates[3];
            if ((Convert.ToInt32(dates[4].Substring(0, 1)) < 10) && dates[4].Substring(1, 2) == ":")
            {
                model.EndTime = "0" + dates[4];

            }
            else
            {
                model.EndTime = dates[4];
            }
            model.DataType = datatype;


            // build model for connecting database
            Models.Test data_base = new Models.Test();
            data_base.gatherDataFromDate(model);

            if (Download != null)
            {
                ExportClientsListToCSV(data_base.list);
            }
            else
            {
                Debug.WriteLine(model.StartDate);
                Debug.WriteLine(model.StartTime);

            }



            return View(data_base);
        }

        public DataPoint GetTemperatureData(string datatype)
        {

            Debug.WriteLine("called GetTemperatureData().");
            List<DataModel> realtime_data = realtime_data_base.getrealtimetemperature();
            //List<DataModel> realtime_data = realtime_data_base.gatherrealtimedata(datatype);

            try
            {
                String new_timestamp = Convert.ToDateTime(realtime_data[0].serverTime).ToString();
                Double new_value = Convert.ToDouble(realtime_data[0].value);

                return new DataPoint(new_timestamp, new_value);
            }
            catch { }
            return new DataPoint(DateTime.Now.ToString(), 0);
        }

        public AccelerationDataPoint GetAccelerationData(string datatype)
        {
            List<DataModel> realtime_data = realtime_data_base.getrealtimeacceleration();

            String new_timestamp = Convert.ToDateTime(realtime_data[0].serverTime).ToString();
            //value for acceleration is in the form of a string --> x: .., y: .., z: ..
            //Thus it needs to be splitted
            String[] new_value = realtime_data[0].value.ToString().Split(',');
            AccelerationDataPoint point = new AccelerationDataPoint(new_timestamp, new_value);

            return point;
        }

        //for donwloading .csv file

        public void ExportClientsListToCSV(List<DataModel> dataList)
        {

            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Sensor ID\",\"Timestamp\",\"Value\",\"DataType\",\"ServerTime\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Exported_Data.csv");
            Response.ContentType = "text/csv";

            foreach (var item in dataList)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                           item.sensorID,
                                           item.timestamp,
                                           item.value,
                                           item.dataType,
                                           item.serverTime));
            }

            Response.Write(sw.ToString());

            Response.End();

        }

    }


}