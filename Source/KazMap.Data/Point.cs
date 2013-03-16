using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KazMap.Data
{
    public class Point
    {
        public double Longitude;
        public double Latitude;

        public Point()
        {
        }

        public Point(Nodes node)
        {
            Longitude = node.Longitude.Value;
            Latitude = node.Latitude.Value;
        }
    }
}
