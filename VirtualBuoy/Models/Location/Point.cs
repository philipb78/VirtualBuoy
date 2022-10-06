using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Point
    {
        public Point()
        {
        }

        public Point(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public double Lat { get; set; }

        public double Lon { get; set; }
    }
}