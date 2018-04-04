using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using AdminLTE_MVC.Models;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AdminLTE_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Models.Test data_base = new Models.Test();
            return View(data_base);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult getDate(string daterange, string Download, string datatype)
        {
            string[] dates = daterange.Split();

            //input date range to model
            Models.DateRangeViewModel model = new Models.DateRangeViewModel();
            model.StartDate = dates[0];
            if ((Convert.ToInt32(dates[1].Substring(0,1)) < 10) && (dates[1].Substring(1,1) == ":"))
            {
                
                model.StartTime = "0" + dates[1];
            
            }
            else {
                model.StartTime = dates[1];
            }
            model.EndDate = dates[3];
            if ((Convert.ToInt32(dates[4].Substring(0,1)) < 10) && dates[4].Substring(1,1) == ":")
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
            List<DataModel> data = data_base.list;


            if (Download != null)    
            {
                ExportClientsListToCSV(data);
            }
            else
            {
                Debug.WriteLine(model.StartDate);
                Debug.WriteLine(model.StartTime);

            }


            
            return View(data_base);
        }
        


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


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}