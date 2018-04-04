using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace AdminLTE_MVC.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "x")]
        public string X;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public double Y;
    }
}