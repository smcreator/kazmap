using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace MapRoutingDemo.ShortPath
{
    class DijkstraSource : IDijkstraSource
    {
        readonly bool _direction, _noHeuristics;
        readonly SqlCommand _intersectionNodeRoads, _pathLeftNodeRightNodes;
        private readonly double _endLon, _endLat;

        public DijkstraSource(SqlCommand intersectionNodeRoads, SqlCommand pathLeftNodeRightNodes,
             bool direction, bool noHeuristics, double endLon, double endLat)
        {
            _intersectionNodeRoads = intersectionNodeRoads;
            _pathLeftNodeRightNodes = pathLeftNodeRightNodes;
            _direction = direction;
            _noHeuristics = noHeuristics;
            _endLon = endLon;
            _endLat = endLat;
        }

        public int CalcHForNode(long id, out double lon, out double lat)
        {
            _intersectionNodeRoads.Parameters[0].Value = id;
            double avgSpeed;
            using (var rdr = _intersectionNodeRoads.ExecuteReader())
            {
                if (!rdr.Read())
                {
                    lon = lat = double.NaN;
                    return Int32.MaxValue;
                }
                try
                {
                    lon = rdr.GetDouble(0);
                    lat = rdr.GetDouble(1);
                    avgSpeed = (double)rdr.GetDecimal(2);
                }
                catch (Exception ex)
                {
                    lon = lat = double.NaN;
                    return Int32.MaxValue;
                }
            }

            return (int)Math.Ceiling(Math.Abs(Distance.Calc(lon, lat, _endLon, _endLat) * 3600) / avgSpeed);
        }

        /// <summary>
        /// TODO: Cache matrix in memory!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<KeyValuePair<long, int>> GetLinks(long id)
        {
            var edges = new List<KeyValuePair<long, int>>();
            _pathLeftNodeRightNodes.Parameters[0].Value = id;
            using (var rdr = _pathLeftNodeRightNodes.ExecuteReader())
            {
                while (rdr.Read())
                {
                    edges.Add(new KeyValuePair<long, int>(rdr.GetInt64(0), rdr.GetInt32(1)));
                }
            }
            return edges;
        }
        public bool Direction { get { return _direction; } }
    }
}
