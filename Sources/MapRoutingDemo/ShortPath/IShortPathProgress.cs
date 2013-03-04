using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapRoutingDemo.ShortPath
{
    interface IShortPathProgress
    {
        void OnStart(long start, double startLon, double startLat, long end, double endLon, double endLat);
        void OnPoint(long point, double lon, double lat);
        void OnEdge(long a, long b, bool direction);
        void OnSelection(long a);
    }
}
