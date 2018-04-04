using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace AdminLTE_MVC.Models
{
    [DataContract]
    public class AccelerationDataPoint
    {
        public AccelerationDataPoint(string x, string[] vector)
        {
            this.X = x;

            //Calculating the acceleration via the 3-coordinate vector
            double[] raw = new double[3];
            for (int i = 0; i < vector.Length; i++)
            {
                String temp = vector[i].Trim().Substring(3);
                raw[i] = Convert.ToDouble(temp);
            }

            this.Y = Math.Sqrt(Math.Pow(raw[0], 2) * Math.Pow(raw[1], 2) * Math.Pow(raw[2], 2));
        }

        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "x")]
        public string X;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public double Y;
    }
}