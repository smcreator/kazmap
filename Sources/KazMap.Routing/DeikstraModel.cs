using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KazMap.DataAccess;

namespace KazMap.Routing
{
    /// <summary>
    /// Используется алгоритм A* для определения оптимального кратчайшего пути
    /// Веса строятся на основании приоритета дорог: primary, secondary и т.д
    /// </summary>
    public class DeikstraModel
    {
        private List<Path> _matrix;

        // The set of nodes already evaluated.
        readonly Dictionary<long, NodeScore> _closedSet;
        // The set of tentative nodes to be evaluated
        readonly Dictionary<long, NodeScore> _openSet;
        // The map of navigated nodes.
        readonly Dictionary<long, long> _cameFrom;

        private Point _endPoint;

        public DeikstraModel(List<Path> matrix)
        {
            _matrix = matrix;

            _closedSet = new Dictionary<long, NodeScore>();
            _openSet = new Dictionary<long, NodeScore>();
            _cameFrom = new Dictionary<long, long>();
        }

        public void Begin(Intersections startIntersection, Point endPoint)
        {
            _endPoint = endPoint;
            _openSet.Add(startIntersection.Id, new NodeScore
            {
                H = GetHValue(startIntersection, endPoint),
                G = 0
            });
        }

        public long SelectNode(out NodeScore score)
        {
            long candidate;
            using (var enumerator = _openSet.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    score = null;
                    return Int64.MaxValue; //< Nothing left!
                }
                candidate = enumerator.Current.Key;
                score = enumerator.Current.Value;
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.F < score.F)
                    {
                        score = enumerator.Current.Value;
                        candidate = enumerator.Current.Key;
                    }
                }
            }
            _openSet.Remove(candidate);
            _closedSet.Add(candidate, score);
            return candidate;
        }

        public void ProcessNode(long id, NodeScore score)
        {
            var edges = GetLinks(id);

            foreach (var edge in edges)
            {
                var rightIdx = edge.Key.Id;
                var distBetween = edge.Value;

                // Ignore nodes already in the closeset
                if (_closedSet.ContainsKey(rightIdx)) continue;

                var tentativeGScore = score.G + distBetween;
                var tentativeIsBetter = false;
                NodeScore yScore;
                if (!_openSet.TryGetValue(rightIdx, out yScore))
                {
                    var hScore = GetHValue(edge.Key, _endPoint);
                    if (hScore == Int32.MaxValue) continue; //< Database error??
                    yScore = new NodeScore { H = hScore };
                    _openSet.Add(rightIdx, yScore);
                    tentativeIsBetter = true;
                }
                else if (tentativeGScore < yScore.G)
                {
                    tentativeIsBetter = true;
                }

                if (tentativeIsBetter)
                {
                    _cameFrom[rightIdx] = id;
                    yScore.G = tentativeGScore;
                    //yScore.F = tentativeGScore + yScore
                }
            }
        }

        public IList<long> GetResult(Intersections endIntersection, bool inReverse)
        {
            var node = endIntersection.Id;
            long c;
            var result = new List<long> { node };

            while (_cameFrom.TryGetValue(node, out c))
            {
                result.Add(c);
                node = c;
            }

            if (!inReverse) result.Reverse();
            return result;
        }

        public IList<KeyValuePair<Intersections, int>> GetLinks(long id)
        {
            return _matrix.Where(m => m.LeftId == id).Select(m => 
                new KeyValuePair<Intersections, int>(m.RightIntersection, m.Weight.Value)).ToList();
        }

        private int GetHValue(Intersections intersection, Point endPoint)
        {
            var averageSpeedOfNodeRoads =
                (from r in KazMapConnection.Read.Roads
                 join w in KazMapConnection.Read.Ways on r.WayId equals w.WayId
                 join wn in KazMapConnection.Read.WayNodes on w.WayId equals wn.WayId
                 where wn.NodeId == intersection.NodeId
                 select r.MaxSpeed.Value).Average();
            var point = new Point(intersection.Node);
            return (int)Math.Ceiling(Math.Abs(
                Distance.Calc(point.Longitude, point.Latitude, endPoint.Longitude, 
                endPoint.Latitude) * 3600) / averageSpeedOfNodeRoads);
        }
    }
}
