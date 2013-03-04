using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KazMap.DataAccess;

namespace KazMap.Routing
{
    public class Route : List<Path>
    {
        public int Time { get; set; }
        public double Distance { get; set; }
    }
}
