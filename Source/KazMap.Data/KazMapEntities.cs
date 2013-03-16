using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KazMap.Data
{
    public partial class KazMapEntities
    {
        public IQueryable<Nodes> GetSurface(Bounds bounds)
        {
            return Nodes.Where(n => bounds.LeftTop.Longitude <= n.Longitude &&
                n.Longitude <= bounds.RightBottom.Longitude &&
                bounds.RightBottom.Latitude <= n.Latitude &&
                n.Latitude <= bounds.LeftTop.Latitude);
        }
    }
}
