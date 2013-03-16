using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KazMap.Data;

namespace KazMap.Routing
{
    public class RoutingService : IRoutingService
    {
        private KazMapEntities _db = new KazMapEntities();

        // CACHE |||||||||||||||||||||||||
        private static List<Path> _matrix;
        private static List<Intersections> _intersections;
        // CACHE |||||||||||||||||||||||||

        public List<Path> Matrix
        {
            get
            {
                if (_matrix == null)
                    _matrix = _db.Path.ToList();
                return _matrix;
            }
        }

        public List<Intersections> Intersections
        {
            get
            {
                if (_intersections == null)
                    _intersections = _db.Intersections.ToList();
                return _intersections;
            }
        }

        public Route GetRoute(Intersections startIntersection, Intersections endIntersection)
        {
            var route = new Route();

            var startPoint = new Point(startIntersection.Node);
            var endPoint = new Point(endIntersection.Node);

            var deikstra = new DeikstraModel(Matrix, Intersections);
            deikstra.Begin(startIntersection, endPoint);

            NodeScore score;
            var node = deikstra.SelectNode(out score);
            while (node != Int64.MaxValue)
            {
                if (node == endIntersection.Id)
                {
                    var result = deikstra.GetResult(endIntersection, false);

                    for (int i = 0; i < result.Count - 1; i++)
                        route.Add(_matrix.First(m => m.LeftIntersectionId == result[i] && 
                            m.RightIntersectionId == result[i + 1]));
                    return route;
                }

                deikstra.ProcessNode(node, score);
                node = deikstra.SelectNode(out score);
            }

            return route;
        }

        public Intersections GetIntersection(double longitude, double latitude)
        {
            var intersections = _db.Intersections.ToList();

            double min = double.MaxValue;
            Intersections result = null;

            foreach (var intersection in intersections)
            {
                var distance = Distance.Calc(longitude, latitude, 
                    intersection.Node.Longitude.Value, intersection.Node.Latitude.Value);
                if (distance < min)
                {
                    min = distance;
                    result = intersection;
                }
            }

            return result;
        }
    }
}
