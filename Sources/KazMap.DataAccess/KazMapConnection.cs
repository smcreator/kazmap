using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KazMap.DataAccess
{
    public partial class KazMapConnection
    {
        private static KazMapConnection _read;

        public static KazMapConnection Read
        {
            get
            {
                if (_read == null)
                    _read = new KazMapConnection();
                return _read;
            }
        }

        public IQueryable<Nodes> Surface(Bounds bounds)
        {
            return Nodes.Where(n => bounds.LeftTop.Longitude <= n.Longitude &&
                n.Longitude <= bounds.RightBottom.Longitude &&
                bounds.RightBottom.Latitude <= n.Latitude &&
                n.Latitude <= bounds.LeftTop.Latitude);
        }
    }
}
