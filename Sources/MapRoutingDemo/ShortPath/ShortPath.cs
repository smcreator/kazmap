using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MapRoutingDemo.ShortPath
{
    class ShortPath
    {
        readonly SqlConnection _dbSource;
        readonly SqlCommand _intersectionNodeRoads, _pathLeftNodeRightNode, _pathRightNodeLeftNode;

        public ShortPath(SqlConnection dbSource)
        {
            _dbSource = dbSource;

            _intersectionNodeRoads = _dbSource.CreateCommand();
            _intersectionNodeRoads.CommandText = @"
select Longitude, Latitude, avg(rd.MaxSpeed * 1.0) avgs 
from Roads rd
inner join WayNodes wn on rd.wayId=wn.wayId
inner join Nodes n on wn.nodeId=n.id
where wn.nodeId=(
    select nodeId 
    from intersections
    where id=@id)
group by Longitude, Latitude";
            _intersectionNodeRoads.Parameters.Add("@id", DbType.Int64);

            _pathLeftNodeRightNode = _dbSource.CreateCommand();
            _pathLeftNodeRightNode.CommandText = @"
select [rightId], weight 
from path
where LeftId = @id and RightId <> LeftId
order by Weight";
            _pathLeftNodeRightNode.Parameters.Add("@id", DbType.Int64);

            _pathRightNodeLeftNode = _dbSource.CreateCommand();
            _pathRightNodeLeftNode.CommandText = @"
select leftId, Weight 
from path
where RightId = @id and RightId <> LeftId
order by Weight";
            _pathRightNodeLeftNode.Parameters.Add("@id", DbType.Int64);
        }

        public bool UseClassicDijkstra { get; set; }
        public bool UseTwoDirection { get; set; }        

        public IList<long> FindPath(long start, long end, IShortPathProgress report)
        {
            return UseTwoDirection ? FindPathBothWay(start, end, report) : FindPathOneWay(start, end, report);
        }

        IList<long> FindPathOneWay(long start, long end, IShortPathProgress report)
        {
            double startLon, startLat, endLon, endLat;

            if (!GetCoord(start, out startLon, out startLat)) throw new Exception("Invalid Start Point");
            if (!GetCoord(end, out endLon, out endLat)) throw new Exception("Invalid End Point");
            var source = new DijkstraSource(_intersectionNodeRoads, 
                _pathLeftNodeRightNode, false, UseClassicDijkstra, endLon, endLat);

            if (report != null) report.OnStart(start, startLon, startLat, end, endLon, endLat);

            var route = new Dijkstra(source, report, start);

            NodeScore selScore;
            var selNode = route.SelectNode(out selScore);
            while (selNode != Int64.MaxValue)
            {
                if (selNode == end)
                {
                    return route.GetPath(end, false);
                }

                route.ProcessNode(selNode, selScore);
                selNode = route.SelectNode(out selScore);
            }

            // Failure
            return new List<long>();
        }


        IList<long> FindPathBothWay(long start, long end, IShortPathProgress report)
        {
            double startLon, startLat, endLon, endLat;

            if (!GetCoord(start, out startLon, out startLat)) throw new Exception("Invalid Start Point");
            if (!GetCoord(end, out endLon, out endLat)) throw new Exception("Invalid End Point");

            if (report != null) report.OnStart(start, startLon, startLat, end, endLon, endLat);

            var routeForward = new Dijkstra(
                new DijkstraSource(_intersectionNodeRoads, _pathLeftNodeRightNode, false, UseClassicDijkstra, endLon, endLat),
                report, start);
            var routeBackward = new Dijkstra(
                new DijkstraSource(_intersectionNodeRoads, _pathRightNodeLeftNode, true, UseClassicDijkstra, startLon, startLat),
                report, end);

            var idx = 0;
            var selection = new Dijkstra[] { routeForward, routeBackward };

            NodeScore selScore;
            var selNode = selection[0].SelectNode(out selScore);
            while (selNode != Int64.MaxValue)
            {
                if (idx == 0 && selNode == end)
                {
                    return selection[0].GetPath(end, false);
                }
                if (idx == 1 && selNode == start)
                {
                    return selection[1].GetPath(start, true);
                }

                selection[idx].ProcessNode(selNode, selScore);

                idx = (idx + 1) & 1;
                if (selection[idx].IsNodeCompleted(selNode))
                {
                    var up = selection[0].GetPath(selNode, false);
                    var down = selection[1].GetPath(selNode, true);
                    using (var itrDown = down.GetEnumerator())
                    {
                        if (itrDown.MoveNext()) while (itrDown.MoveNext()) up.Add(itrDown.Current);
                    }
                    return up;
                }
                selNode = selection[idx].SelectNode(out selScore);
            }

            // Failure
            return new List<long>();
        }

        bool GetCoord(long id, out double lon, out double lat)
        {
            double notUsed;
            return GetCoord(id, out lon, out lat, out notUsed);
        }

        bool GetCoord(long id, out double lon, out double lat, out double avgSpeed)
        {
            _intersectionNodeRoads.Parameters[0].Value = id;
            using (var rdr = _intersectionNodeRoads.ExecuteReader())
            {
                if (!rdr.Read())
                {
                    avgSpeed = lon = lat = double.NaN;
                    return false;
                }
                try
                {
                    lon = rdr.GetDouble(0);
                    lat = rdr.GetDouble(1);
                    avgSpeed = (double)rdr.GetDecimal(2);
                }
                catch (Exception ex)
                {
                    avgSpeed = lon = lat = double.NaN;
                    return false;
                }
            }
            return true;
        }        
    }
}