using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapRoutingDemo.ShortPath
{
    class Dijkstra
    {
        // The set of nodes already evaluated.
        readonly Dictionary<long, NodeScore> _closedset;
        // The set of tentative nodes to be evaluated
        readonly Dictionary<long, NodeScore> _openset;
        // The map of navigated nodes.
        readonly Dictionary<long, long> _cameFrom;
        // Data source
        readonly IDijkstraSource _source;
        // Progress report
        readonly IShortPathProgress _report;

        public Dijkstra(IDijkstraSource source, IShortPathProgress report, long startId)
        {
            _closedset = new Dictionary<long, NodeScore>();
            _openset = new Dictionary<long, NodeScore>();
            _cameFrom = new Dictionary<long, long>();
            _source = source;
            _report = report;

            double lon, lat;
            var hScore = source.CalcHForNode(startId, out lon, out lat);
            _openset.Add(startId, new NodeScore
                {
                    H = hScore,
                    G = 0 // hScore
                });
        }

        public long SelectNode(out NodeScore score)
        {
            long cand;
            using (var @enum = _openset.GetEnumerator())
            {
                if (!@enum.MoveNext())
                {
                    score = null;
                    return Int64.MaxValue; //< Nothing left!
                }
                cand = @enum.Current.Key;
                score = @enum.Current.Value;
                while (@enum.MoveNext())
                {
                    if (@enum.Current.Value.F < score.F)
                    {
                        score = @enum.Current.Value;
                        cand = @enum.Current.Key;
                    }
                }
            }
            _openset.Remove(cand);
            _closedset.Add(cand, score);
            return cand;
        }

        public void ProcessNode(long id, NodeScore score)
        {
            var edges = _source.GetLinks(id);

            foreach (var edge in edges)
            {
                var rightIdx = edge.Key;
                var distBetween = edge.Value;

                // Ignore nodes already in the closeset
                if (_closedset.ContainsKey(rightIdx)) continue;

                var tentativeGScore = score.G + distBetween;
                var tentativeIsBetter = false;
                NodeScore yScore;
                if (!_openset.TryGetValue(rightIdx, out yScore))
                {
                    double lon, lat;
                    var hScore = _source.CalcHForNode(rightIdx, out lon, out lat);
                    if (hScore == Int32.MaxValue) continue; //< Database error??
                    if (_report != null) _report.OnPoint(rightIdx, lon, lat);
                    yScore = new NodeScore { H = hScore };
                    _openset.Add(rightIdx, yScore);
                    tentativeIsBetter = true;
                }
                else if (tentativeGScore < yScore.G)
                {
                    tentativeIsBetter = true;
                }

                if (_report != null) _report.OnEdge(id, rightIdx, _source.Direction);
                if (tentativeIsBetter)
                {
                    _cameFrom[rightIdx] = id;
                    yScore.G = tentativeGScore;
                    //yScore.F = tentativeGScore + yScore.H;
                }
            }
        }

        public bool IsNodeCompleted(long id)
        {
            return _closedset.ContainsKey(id);
        }

        public IList<long> GetPath(long endPoint, bool inReverse)
        {
            var node = endPoint;
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
    }
}
