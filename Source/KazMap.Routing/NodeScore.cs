using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KazMap.Routing
{
    public class NodeScore
    {
        public int G, H;
        public int F
        {
            get { return G + H; } 
        }

        public override string ToString()
        {
            return string.Format("G={0}/H={1}/F={2}", G, H, F);
        }
    }
}
