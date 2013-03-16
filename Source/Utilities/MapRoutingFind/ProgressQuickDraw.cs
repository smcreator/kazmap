using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using MapRoutingDemo.ShortPath;

namespace MapRoutingDemo
{
    class ProgressQuickDraw : IShortPathProgress
    {
        readonly object _lock = new object();
        GraphicsPath _startEndPoints,_pointAndEdges,_solution;


        Dictionary<long, PointF> _points;
        PointF _topBox,_bottomBox,_topRef,_start,_end;
        long _totNode, _totEdge, _selection;

        public Bitmap GetCurrent(int width, int height,out string stats)
        {
            lock (_lock)
            {
                stats = string.Format("Total nodes: {0} / edges: {1}", _totNode, _totEdge);

                var ret = new Bitmap(Math.Max(100,width), Math.Max(100,height));

                if (_totNode != 0)
                {
                    using (var fx = Graphics.FromImage(ret))
                    {
                        var mapDX = (_bottomBox.X - _topBox.X);
                        var mapDY = (_bottomBox.Y - _topBox.Y);

                        fx.TranslateTransform(-_topBox.X, -_topBox.Y, MatrixOrder.Prepend);
                        fx.ScaleTransform(ret.Width / mapDX, ret.Height / mapDY, MatrixOrder.Append);

                        if (_pointAndEdges != null) fx.DrawPath(Pens.Gray, _pointAndEdges);
                        if (_startEndPoints != null) fx.FillPath(Brushes.Red, _startEndPoints);
                        if (_solution != null) fx.DrawPath(Pens.Green, _solution);
                    }
                }
                return ret;
            }
        }

        public void DrawSolution(IList<long> solution)
        {
            lock (_lock)
            {
                _solution = new GraphicsPath();
                var @enum = solution.GetEnumerator();
                if (!@enum.MoveNext()) return;

                PointF p,c;
                if (!_points.TryGetValue(@enum.Current, out p)) return;
                _solution.AddRectangle(new RectangleF(p.X - 2, p.Y - 2, 4, 4));

                while (@enum.MoveNext())
                {
                    if (!_points.TryGetValue(@enum.Current, out c)) continue;
                    _solution.AddLine(p,c);
                    p = c;
                    _solution.AddRectangle(new RectangleF(p.X - 2, p.Y - 2, 4, 4));
                }
            }
        }

        public void OnStart(long start, double startLon, double startLat, long end, double endLon, double endLat)
        {
            _start = new PointF(
                (float) MercatorProjection.lonToX(startLon),
                (float) MercatorProjection.latToY(startLat)
                );
            _end = new PointF(
                (float)MercatorProjection.lonToX(endLon),
                (float)MercatorProjection.latToY(endLat)
                );

            lock (_lock)
            {
                _startEndPoints = new GraphicsPath();
                _pointAndEdges = new GraphicsPath();

                _topRef = new PointF(Math.Min(_start.X, _end.X), Math.Max(_start.Y, _end.Y));
                _bottomBox = new PointF(Math.Max(_start.X, _end.X) - _topRef.X, _topRef.Y - Math.Min(_start.Y, _end.Y));
                _topBox = new PointF(-_bottomBox.X * .01f, -_bottomBox.Y * .01f);
                _bottomBox.X += _bottomBox.X*.01f;
                _bottomBox.Y += _bottomBox.Y*.01f;

                _start = new PointF(_start.X - _topRef.X, _topRef.Y - _start.Y);
                _end = new PointF(_end.X - _topRef.X, _topRef.Y - _end.Y);

                _points = new Dictionary<long, PointF> { { start, _start }, { end, _end } };
                _totNode = 2;
                _totEdge = 0;

                _startEndPoints.StartFigure();
                _startEndPoints.AddEllipse(new RectangleF(_start.X - 100, _start.Y - 100, 100, 100));
                _startEndPoints.AddEllipse(new RectangleF(_end.X - 100, _end.Y - 100, 100, 100));
                _startEndPoints.CloseFigure();
            }
        }

        public void OnPoint(long point, double lon, double lat)
        {
            var proj = new PointF(
                (float)MercatorProjection.lonToX(lon) - _topRef.X,
                _topRef.Y - (float)MercatorProjection.latToY(lat)
                );

            lock (_lock)
            {
                if(_points.ContainsKey(point)) return;

                _points.Add(point, proj);

                if(proj.X<_topBox.X || proj.Y<_topBox.Y)
                    _topBox = new PointF(Math.Min(proj.X, _topBox.X), Math.Min(proj.Y, _topBox.Y));
                if(proj.X>_bottomBox.X || proj.Y>_bottomBox.Y)
                    _bottomBox = new PointF(Math.Max(proj.X, _bottomBox.X), Math.Max(proj.Y, _bottomBox.Y));
                ++_totNode;
            }
        }

        public void OnEdge(long a, long b,bool notUsedYet)
        {
            lock (_lock)
            {
                PointF pA, pB;
                if(!_points.TryGetValue(a,out pA)) return;
                if(!_points.TryGetValue(b,out pB)) return;

                _pointAndEdges.StartFigure();
                _pointAndEdges.AddLine(pA, pB);
                _pointAndEdges.CloseFigure();
                ++_totEdge;
            }
        }

        public void OnSelection(long a)
        {
            lock (_lock)
            {
                _selection = a;
            }
        }
    }
}
