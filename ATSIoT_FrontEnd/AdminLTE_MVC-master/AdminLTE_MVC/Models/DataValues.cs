using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE_MVC.Models
{
    public class DataValues
    {
            public string sensorID { get; set; }
            public string timestamp { get; set; }
            public string value { get; set; }
            public string dataType { get; set; }
            public string serverTime { get; set; }
    }
}