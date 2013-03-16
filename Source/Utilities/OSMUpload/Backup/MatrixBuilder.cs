using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace OSMUpload
{
    class MatrixBuilder : IDisposable
    {
        static readonly Regex RxSpeed = new Regex(@"^\s*(?<speed>[0-9.]+)\s*(?<mph>mph)?\s*$",
                                                               RegexOptions.IgnoreCase | RegexOptions.Compiled);


        readonly SQLiteConnection _dbSource;
        long _nbIntersections;
        readonly Dictionary<string, int> _typeIds;
        readonly Dictionary<string, bool> _oneWayStr;
        readonly Dictionary<string, int> _speedByWayType;
        SQLiteCommand _connection, _point2Point;

        MatrixBuilder(SQLiteConnection dbSource)
        {
            _dbSource = dbSource;
            using (var cmd = _dbSource.CreateCommand())
            {
                cmd.CommandText = @"
DROP TABLE IF EXISTS Road;
DROP TABLE IF EXISTS Intersection;
DROP TABLE IF EXISTS Path;

CREATE TABLE Road(wayid not null primary key,category text not null,maxspeed integer not null);

CREATE TABLE Intersection(id integer not null primary key,nodeId integer);
CREATE INDEX Intersection_NodeID ON Intersection(nodeId);
CREATE TABLE Path(left integer not null,right integer not null,weight integer, PRIMARY KEY (left,right));
CREATE INDEX PathLefts ON Path(Left);
CREATE INDEX PathRights ON Path(Right);

PRAGMA count_changes=OFF;
";
                cmd.ExecuteNonQuery();
            }

            _typeIds = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            using (var cmd = _dbSource.CreateCommand())
            {
                cmd.CommandText = @"SELECT name,id FROM tagtype";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        _typeIds.Add(rdr.GetString(0), rdr.GetInt32(1));
                    }
                }
            }

            _connection = _dbSource.CreateCommand();
            _connection.CommandText = @"
select l.wayID, l.pos leftPos, r.pos rightPos, (select value from WayTAG where wayId=l.wayID and type=@oneway) oneway
from WayNODE l inner join WayNODE r on l.wayId=r.wayId and 
l.nodeId=@leftNode and r.nodeId=@rightNode
";
            _connection.Parameters.Add("@leftNode", DbType.Int64);
            _connection.Parameters.Add("@rightNode", DbType.Int64);
            _connection.Parameters.AddWithValue("@oneway", _typeIds["oneway"]);

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


            _point2Point = _dbSource.CreateCommand();
            _point2Point.CommandText = @"
select nd.MinX,nd.MinY 
from WayNODE inner join NODES nd on nodeID = id 
where wayId=@wayid and pos>=@top and pos<=@bottom
";
            _point2Point.Parameters.Add("@wayId", DbType.Int64);
            _point2Point.Parameters.Add("@top", DbType.Int32);
            _point2Point.Parameters.Add("@bottom", DbType.Int32);
        }

        public void Dispose()
        {
            if (_connection != null) { _connection.Dispose(); _connection = null; }
            if (_point2Point != null) { _point2Point.Dispose(); _point2Point = null; }
        }

        void UploadRoads(NotifyProgressDlg progressDlg)
        {
            var tHighway = _typeIds["highway"];

            using (var src = _dbSource.CreateCommand())
            using (var upl = _dbSource.CreateCommand())
            {
                src.CommandText = string.Format(@"
SELECT w.wayID,t.type,t.value,
(select value from WayTAG where wayId=w.wayID and type={2}) maxspeed
FROM Way w
INNER JOIN WayTAG t on w.wayID=t.wayID and t.type in ({0},{1})  
WHERE
(t.type={0} and t.value in ('living_street','motorway','motorway_link','primary','primary_link',
'residential','road','secondary','secondary_link','tertiary','tertiary_link','trunk',
'trunk_link')) or t.type={1}", tHighway, _typeIds["junction"], _typeIds["maxspeed"]);
                src.Prepare();

                upl.CommandText = @"INSERT OR REPLACE INTO Road VALUES (@wayid,@category,@maxspeed)";
                var pWid = upl.Parameters.Add("@wayid",     DbType.Int64);
                var pCat = upl.Parameters.Add("@category",  DbType.String);
                var pSpd = upl.Parameters.Add("@maxspeed",  DbType.Int32);

                var trs = _dbSource.BeginTransaction();
                upl.Prepare();
                upl.Transaction = trs;

                using (var rdr = src.ExecuteReader())
                {
                    for (var idx = 1; rdr.Read();++idx)
                    {
                        if (progressDlg != null && (idx % 1000) == 0) progressDlg(idx + " roads processed");

                        pWid.Value = rdr.GetInt64(0);

                        var wtype = rdr.GetInt32(1);
                        var roadType= wtype==tHighway?rdr.GetString(2):"junction";
                        pCat.Value = roadType;

                        if(!rdr.IsDBNull(3))
                        {
                            var strMaxSpeed = rdr.GetString(3);
                            var m = RxSpeed.Match(strMaxSpeed);
                            if(m.Success)
                            {
                                double speed;
                                if(double.TryParse(m.Groups["speed"].Value,out speed))
                                {
                                    if (m.Groups["mph"] != null) speed *= 1.609344;
                                    pSpd.Value = (int)speed;
                                    goto haveSpecifiedSpeed;
                                }
                            }
                        }
                        // else compute the speed from the type
                        {
                            int speed;
                            if (!_speedByWayType.TryGetValue(roadType, out speed)) speed = 15;
                            pSpd.Value = speed;
                        }
                    haveSpecifiedSpeed:
                        upl.ExecuteNonQuery();
                    }
                }

                trs.Commit();
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
            using (var src = _dbSource.CreateCommand())
            {
                src.CommandText = @"
select w.nodeId from  WayNODE w inner join WayNODE w2 on w2.nodeId=w.nodeId
where w.wayID in (select wayID from Road) and w2.wayID in (select wayID from Road) and w.wayID<>w2.wayID
group by w.nodeId";

                using (var rdr = src.ExecuteReader())
                using (var upl = _dbSource.CreateCommand())
                {
                    upl.CommandText = "INSERT INTO Intersection VALUES(@position,@nodeId)";
                    var uplPos = upl.Parameters.Add("@position", DbType.Int64);
                    var uplNod = upl.Parameters.Add("@nodeId", DbType.Int64);
                    upl.Prepare();

                    var trs = _dbSource.BeginTransaction();
                    upl.Transaction = trs;

                    for (_nbIntersections = 0L; rdr.Read(); ++_nbIntersections)
                    {
                        if (progressDlg != null && (_nbIntersections % 1000) == 0 && _nbIntersections!=0) progressDlg(_nbIntersections + " intersections processed");
                        uplPos.Value = _nbIntersections;
                        uplNod.Value = rdr.GetInt64(0);
                        upl.ExecuteNonQuery();
                    }
                    trs.Commit();
                }
            }
        }

        int CalculateConnection(long wayId, int lPos, int rPos, int speed)
        {
            if (lPos > rPos)
            {
                var t = rPos;
                rPos = lPos;
                lPos = t;
            }

            _point2Point.Parameters[0].Value = wayId;
            _point2Point.Parameters[1].Value = lPos;
            _point2Point.Parameters[2].Value = rPos;

            double distance = 0;
            using (var rdr = _point2Point.ExecuteReader())
            {
                if (!rdr.Read()) return Int32.MaxValue; //!!???
                double lastX = rdr.GetDouble(0), lastY = rdr.GetDouble(1);
                while (rdr.Read())
                {
                    var cx = rdr.GetDouble(0);
                    var cy = rdr.GetDouble(1);
                    distance += Math.Abs(Distance.Calc(lastX, lastY, cx, cy));
                    lastX = cx; lastY = cy;
                }
            }

            return (int)Math.Ceiling((distance * 3600) / speed);
        }

        void SetupMatrix(NotifyProgressDlg progressDlg)
        {
            progressDlg("Preparing intersection matrix generation ...");
            using (var insRepPath = _dbSource.CreateCommand())
            {
                insRepPath.CommandText = @"INSERT OR REPLACE INTO Path (left,right,weight) VALUES(@left,@right,@weight)";
                var pL = insRepPath.Parameters.Add("@left", DbType.Int64);
                var pR = insRepPath.Parameters.Add("@right", DbType.Int64);
                var pW = insRepPath.Parameters.Add("@weight", DbType.Int32);
                insRepPath.Prepare();

                var trs = _dbSource.BeginTransaction();
                insRepPath.Transaction = trs;

                using (var cmd = _dbSource.CreateCommand())
                {
                    cmd.CommandText =
                        string.Format(@"
select l.wayID, l.pos leftPos, r.pos rightPos, 
(select value from WayTAG where wayId=l.wayID and type={0}) oneway,
rd.maxspeed,
(select id from Intersection where nodeId=l.nodeId) leftIdx,
(select id from Intersection where nodeId=r.nodeId) rightIdx
from WayNODE l inner join WayNODE r on l.wayId=r.wayId and 
l.nodeId in (SELECT nodeID from Intersection) and r.nodeId in (SELECT nodeID from Intersection)
and l.nodeId<>r.nodeId
inner join Road rd on l.wayId = rd.wayId
", _typeIds["oneway"]);
                    var disp = 0L;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        var rows = 0;
                        for (; rdr.Read(); ++disp)
                        {
                            if (progressDlg!=null && (disp % 1000) == 0 && disp!=0) progressDlg(disp + " connections processed");
                            var oneWay = rdr.IsDBNull(3) ? false : _oneWayStr.ContainsKey(rdr.GetString(3));
                            var speed = rdr.GetInt32(4);
                            var lPos = rdr.GetInt32(1);
                            var rPos = rdr.GetInt32(2);
                            if (oneWay && lPos > rPos) continue; //< Can go reverse of traffic!
                            var wayId = rdr.GetInt64(0);
                            var d = CalculateConnection(wayId, lPos, rPos, speed);
                            if (d != Int32.MaxValue)
                            {
                                pL.Value = rdr.GetInt64(5);
                                pR.Value = rdr.GetInt64(6);
                                pW.Value = d;
                                insRepPath.ExecuteNonQuery();

                                ++rows;
                                if (rows > 10000)
                                {
                                    trs.Commit();
                                    trs = _dbSource.BeginTransaction();
                                    insRepPath.Transaction = trs;
                                    rows = 0;
                                }
                            }
                        }
                    }
                    trs.Commit();
                }
            }
        }

        public delegate void NotifyProgressDlg(string text);

        static public void Build(SQLiteConnection backend, NotifyProgressDlg progressDlg)
        {
            using (var mb = new MatrixBuilder(backend))
            {
                mb.UploadRoads(progressDlg);
                mb.UploadIntersection(progressDlg);
                mb.SetupMatrix(progressDlg);
            }
        }
    }
}