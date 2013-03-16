using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using KazMap.Data;
using System.Linq;

namespace OSMUpload
{
    class MatrixBuilder : IDisposable
    {
        static readonly Regex maxSpeedRegex = new Regex(@"^\s*(?<speed>[0-9.]+)\s*(?<mph>mph)?\s*$",
                                                               RegexOptions.IgnoreCase | RegexOptions.Compiled);


        readonly SqlConnection _dbSource;
        long _intersectionIndex;
        readonly Dictionary<string, long> _tagTypes;
        readonly Dictionary<string, bool> _oneWayStr;
        readonly Dictionary<string, int> _speedByWayType;
        SqlCommand _edge, _wayPointRange;
        private KazMapEntities _db = new KazMapEntities();

        MatrixBuilder(SqlConnection dbSource)
        {
            _dbSource = dbSource;
//            using (var cmd = _dbSource.CreateCommand())
//            {
//                cmd.CommandText = @"
//DROP TABLE IF EXISTS Road;
//DROP TABLE IF EXISTS Intersection;
//DROP TABLE IF EXISTS Path;
//
//CREATE TABLE Road(wayid not null primary key,category text not null,maxspeed integer not null);
//
//CREATE TABLE Intersection(id integer not null primary key,nodeId integer);
//CREATE INDEX Intersection_NodeID ON Intersection(nodeId);
//CREATE TABLE Path(left integer not null,right integer not null,weight integer, PRIMARY KEY (left,right));
//CREATE INDEX PathLefts ON Path(Left);
//CREATE INDEX PathRights ON Path(Right);
//
//PRAGMA count_changes=OFF;
//";
//                cmd.ExecuteNonQuery();
//            }

            _tagTypes = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
            _db.TagTypes.ToList().ForEach(i =>
                {
                    _tagTypes.Add(i.Name, i.Id);
                });
            //using (var cmd = _dbSource.CreateCommand())
            //{
            //    cmd.CommandText = @"SELECT Name, Id FROM TagTypes";
            //    using (var rdr = cmd.ExecuteReader())
            //    {
            //        while (rdr.Read())
            //        {
            //            _tagTypes.Add(rdr.GetString(0), rdr.GetInt64(1));
            //        }
            //    }
            //}

            _edge = _dbSource.CreateCommand();
            _edge.CommandText = @"
select l.WayID, 
    l.Position leftPos, 
    r.Position rightPos, 
    (select value 
        from WayTags 
        where WayId=l.WayID and TypeId=@oneway) oneway
from WayNodes l 
inner join WayNodes r on l.WayId=r.WayId and l.NodeId=@leftNode and r.NodeId=@rightNode
";
            _edge.Parameters.Add("@leftNode", DbType.Int64);
            _edge.Parameters.Add("@rightNode", DbType.Int64);
            _edge.Parameters.AddWithValue("@oneway", _tagTypes["oneway"]);

            _oneWayStr = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase)
                             {
                                 {"yes", true},
                                 {"true", true},
                                 {"1", true}
                             };

            _speedByWayType = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
                                  {
                                      {"living_street",10 },  // A street where pedestrians have priority over cars, children can play on the street, maximum speed is low
                                      {"motorway",120 },
                                      {"motorway_link", 60 }, // The link roads (sliproads/ramps) leading to/from a motorway from/to a motorway or lower class highway.
                                      {"primary", 90 },
                                      {"primary_link", 50 },
                                      {"residential", 40 },   // Roads accessing or around residential areas but which are not a classified or unclassified highway.
                                      {"road", 50 },          // A road of unknown classification.
                                      {"secondary",   80 },
                                      {"secondary_link", 50 },
                                      {"tertiary", 75 },
                                      {"tertiary_link", 50 },
                                      {"trunk", 100 },        // Important roads that aren't motorways.
                                      {"trunk_link", 60 }
                                  };


            _wayPointRange = _dbSource.CreateCommand();
            _wayPointRange.CommandText = @"
select nd.Longitude, nd.Latitude 
from WayNodes 
inner join Nodes nd on WayNodes.nodeID = nd.id 
where WayId=@wayid and @top <= Position and Position<=@bottom
";
            _wayPointRange.Parameters.Add("@wayId", DbType.Int64);
            _wayPointRange.Parameters.Add("@top", DbType.Int32);
            _wayPointRange.Parameters.Add("@bottom", DbType.Int32);
        }

        public void Dispose()
        {
            if (_edge != null) { _edge.Dispose(); _edge = null; }
            if (_wayPointRange != null) { _wayPointRange.Dispose(); _wayPointRange = null; }
        }

        static public void Build(SqlConnection backend, NotifyProgressDlg progressDlg)
        {
            using (var mb = new MatrixBuilder(backend))
            {
                mb.UploadRoads(progressDlg);
                mb.UploadIntersection(progressDlg);
                mb.SetupMatrix(progressDlg);
                mb.UpdateIntersectionsAverageSpeed();
            }
        }

        void UploadRoads(NotifyProgressDlg progressDlg)
        {
            var tHighway = _tagTypes["highway"];

            using (var src = _dbSource.CreateCommand())
            using (var saveRoad = _dbSource.CreateCommand())
            {
                src.CommandText = string.Format(@"
SELECT w.WayID,
    t.TypeId, t.Value,
    (select Value 
        from WayTags 
        where WayId=w.WayID and TypeId={2}) maxspeed
FROM Ways w
INNER JOIN WayTags t on w.WayID=t.WayID and t.TypeId in ({0},{1})
WHERE (t.TypeId={0} and 
    t.Value in ('living_street','motorway','motorway_link','primary','primary_link',
        'residential','road','secondary','secondary_link','tertiary','tertiary_link','trunk',
        'trunk_link')) or t.TypeId={1}", tHighway, _tagTypes["junction"], _tagTypes["maxspeed"]);

                //saveRoad.CommandText = @"INSERT INTO Roads VALUES (@wayid,@category,@maxspeed)";
                //var wayId = saveRoad.Parameters.Add("@wayid",     DbType.Int64);
                //var category = saveRoad.Parameters.Add("@category",  DbType.String);
                //var pSpd = saveRoad.Parameters.Add("@maxspeed",  DbType.Int32);                

                //var trs = _dbSource.BeginTransaction();
                //upl.Transaction = trs;

                using (var rdr = src.ExecuteReader())
                {                    
                    for (var idx = 1; rdr.Read();++idx)
                    {                        
                        if (progressDlg != null && (idx % 1000) == 0) progressDlg(idx + " roads processed");

                        var road = new Roads();
                        //wayId.Value = rdr.GetInt64(0);
                        road.WayId = rdr.GetInt64(0);
                        var wayType = rdr.GetInt64(1);
                        var roadType = wayType == tHighway ? rdr.GetString(2) : "junction";
                        //category.Value = roadType;
                        road.Category = roadType;

                        if(!rdr.IsDBNull(3))
                        {
                            var maxSpeed = rdr.GetString(3);
                            var m = maxSpeedRegex.Match(maxSpeed);
                            if(m.Success)
                            {
                                double speed;
                                if(double.TryParse(m.Groups["speed"].Value,out speed))
                                {
                                    if (m.Groups["mph"] != null) speed *= 1.609344;
                                    //pSpd.Value = (int)speed;
                                    road.MaxSpeed = (int)speed;
                                    goto haveSpecifiedSpeed;
                                }
                            }
                        }
                        // else compute the speed from the type
                        {
                            int speed;
                            if (!_speedByWayType.TryGetValue(roadType, out speed)) speed = 15;
                            //pSpd.Value = speed;
                            road.MaxSpeed = speed;
                        }
                    haveSpecifiedSpeed:
                        //saveRoad.ExecuteNonQuery();
                        _db.Roads.AddObject(road);
                        _db.SaveChanges();
                    }
                }

                //trs.Commit();
            }

        }

        /*
select w.nodeId 
from  WayNODE w 
inner join Road r on w.wayID = r.wayID
inner join WayNODE w2 on w2.nodeId=w.nodeId and w.wayID<>w2.wayID
where w2.wayID in (select wayID from Road)
group by w.nodeId
limit 1000         */
        void UploadIntersection(NotifyProgressDlg progressDlg)
        {
            progressDlg("Preparing intersection detection ...");
            using (var intersectedNodes = _dbSource.CreateCommand())
            {
                intersectedNodes.CommandText = @"
select w.NodeId 
from WayNodes w 
inner join WayNodes w2 on w2.nodeId=w.nodeId
where w.wayID in 
    (select wayID 
        from Roads) and w2.wayID in 
    (select wayID 
        from Roads) and w.wayID <> w2.wayID
group by w.nodeId";

                using (var rdr = intersectedNodes.ExecuteReader())
                using (var saveIntersection = _dbSource.CreateCommand())
                {
                    saveIntersection.CommandText = "INSERT INTO Intersections(id,NodeId) VALUES(@id,@nodeId)";
                    var position = saveIntersection.Parameters.Add("@id", DbType.Int64);
                    var nodeId = saveIntersection.Parameters.Add("@nodeId", DbType.Int64);

                    //var trs = _dbSource.BeginTransaction();
                    //upl.Transaction = trs;

                    for (_intersectionIndex = 0L; rdr.Read(); ++_intersectionIndex)
                    {
                        if (progressDlg != null && (_intersectionIndex % 1000) == 0 && _intersectionIndex!=0) progressDlg(_intersectionIndex + " intersections processed");
                        position.Value = _intersectionIndex;
                        nodeId.Value = rdr.GetInt64(0);
                        saveIntersection.ExecuteNonQuery();
                    }
                    //trs.Commit();
                }
            }
        }        

        /// <summary>
        /// Матрица считает дороги от пересечения к пересечению
        /// Лево - левый узел пересечения дорог
        /// Право - правый узел пересечения дорог
        /// Вес - расстояние от Лево до Право
        /// </summary>
        /// <param name="progressDlg"></param>
        void SetupMatrix(NotifyProgressDlg progressDlg)
        {
            progressDlg("Preparing intersection matrix generation ...");
            using (var savePath = _dbSource.CreateCommand())
            {
                savePath.CommandText = @"INSERT INTO Path (LeftIntersectionId,RightIntersectionId,[weight], roadId,LeftNodeId,RightNodeId,Distance) VALUES(@left,@right,@weight,@roadId,@leftNodeId,@rightNodeId,@Distance)";
                var left = savePath.Parameters.Add("@left", DbType.Int64);
                var right = savePath.Parameters.Add("@right", DbType.Int64);
                var weight = savePath.Parameters.Add("@weight", DbType.Int32);
                var roadId = savePath.Parameters.Add("@roadId", DbType.Int64);
                var leftNodeId = savePath.Parameters.Add("@leftNodeId", DbType.Int64);
                var rightNodeId = savePath.Parameters.Add("@rightNodeId", DbType.Int64);
                var dist = savePath.Parameters.Add("@Distance", DbType.Int64);
                //var trs = _dbSource.BeginTransaction();
                //insRepPath.Transaction = trs;

                using (var matrix = _dbSource.CreateCommand())
                {
                    matrix.CommandText =
                        string.Format(@"
select l.wayID, 
    l.position leftPos, r.position rightPos, 
    (select value 
    from WayTags 
    where wayId=l.wayID and TypeId={0}) oneway,
    rd.maxspeed,
    (select id 
    from Intersections
    where nodeId=l.nodeId) leftIdx,
    (select id 
    from Intersections 
    where nodeId=r.nodeId) rightIdx,
    rd.id roadId,
    l.nodeId leftNodeId,
    r.nodeId rightNodeId,
    rd.Category category
from WayNodes l 
    inner join WayNodes r on l.wayId=r.wayId and 
        l.nodeId in 
            (SELECT nodeID 
            from Intersections)
        and r.nodeId in
            (SELECT nodeID 
            from Intersections)
        and l.nodeId <> r.nodeId
inner join Roads rd on l.wayId = rd.wayId
", _tagTypes["oneway"]);
                    var intersectionIndex = 0L;
                    using (var rdr = matrix.ExecuteReader())
                    {
                        var rows = 0;
                        for (; rdr.Read(); ++intersectionIndex)
                        {
                            if (progressDlg!=null && (intersectionIndex % 1000) == 0 && intersectionIndex!=0) progressDlg(intersectionIndex + " connections processed");
                            var wayId = rdr.GetInt64(0);
                            var leftPos = rdr.GetInt32(1);
                            var rightPos = rdr.GetInt32(2);
                            var oneWay = rdr.IsDBNull(3) ? false : _oneWayStr.ContainsKey(rdr.GetString(3));
                            var speed = rdr.GetInt32(4);
                            if (oneWay && leftPos > rightPos) 
                                continue; //< Can go reverse of traffic!

                            long valueLeft = rdr.GetInt64(5);
                            long valueRight = rdr.GetInt64(6);

                            if (valueLeft == 695 && valueRight == 619)
                            {
                            }

                            var distance = CalculateDistance(wayId, leftPos, rightPos, speed);
                            var time = CalculateTime(wayId, leftPos, rightPos, speed);
                            if (time != Int32.MaxValue)
                            {                                
                                left.Value = rdr.GetInt64(5);
                                right.Value = rdr.GetInt64(6);
                                var w = time;

                                string category = rdr.GetString(10);
                                if (category == "primary")
                                    w = w / 3;
                                else
                                    w = w / 2;

                                weight.Value = w;
                                roadId.Value = rdr.GetInt64(7);
                                leftNodeId.Value = rdr.GetInt64(8);
                                rightNodeId.Value = rdr.GetInt64(9);
                                dist.Value = distance;
                                savePath.ExecuteNonQuery();

                                ++rows;
                                if (rows > 10000)
                                {
                                    //trs.Commit();
                                    //trs = _dbSource.BeginTransaction();
                                    //insRepPath.Transaction = trs;
                                    rows = 0;
                                }
                            }
                        }
                    }
                    //trs.Commit();
                }
            }
        }

        void UpdateIntersectionsAverageSpeed()
        {
            foreach (var intersection in _db.Intersections)
            {
                var averageSpeed = (
                    from r in _db.Roads
                    join w in _db.Ways on r.WayId equals w.WayId
                    join wn in _db.WayNodes on w.WayId equals wn.WayId
                    where wn.NodeId == intersection.NodeId
                    select r.MaxSpeed.Value).Average();
                intersection.AverageSpeed = averageSpeed;                
            }
            _db.SaveChanges();
        }

        double CalculateDistance(long wayId, long leftPos, long rightPos, int speed)
        {
            if (leftPos > rightPos)
            {
                var t = rightPos;
                rightPos = leftPos;
                leftPos = t;
            }

            _wayPointRange.Parameters[0].Value = wayId;
            _wayPointRange.Parameters[1].Value = leftPos;
            _wayPointRange.Parameters[2].Value = rightPos;

            double distance = 0;
            using (var rdr = _wayPointRange.ExecuteReader())
            {
                if (!rdr.Read())
                {
                    throw new Exception();
                    //return Int32.MaxValue; //!!???
                }
                double x = rdr.GetDouble(0), y = rdr.GetDouble(1);
                while (rdr.Read())
                {
                    var cx = rdr.GetDouble(0);
                    var cy = rdr.GetDouble(1);
                    distance += Math.Abs(Distance.Calc(x, y, cx, cy));
                    x = cx; y = cy;
                }
            }

            return distance;
        }

        int CalculateTime(long wayId, long leftPos, long rightPos, int speed)
        {

            var distance = CalculateDistance(wayId, leftPos, rightPos, speed);
            return (int)Math.Ceiling((distance * 3600) / speed);
        }

        public delegate void NotifyProgressDlg(string text);        
    }
}