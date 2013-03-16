using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapRoutingDemo
{
    class MapDrawer
    {
        //const int TargetSize = 1280;
        readonly double _top, _left, _right, _bottom;
        readonly int _zoom;
        readonly string _basePage;
        readonly int _pxWidth, _pxHeight, _lftTile, _topTile;
        readonly StringBuilder _vectorGraphics;

        static void WorldToTilePos(double lon, double lat, int zoom, out double X,out double Y)
        {
            X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
        }

        static void TileToWorldPos(double X, double Y, int zoom, out double lon, out double lat)
        {
            double n = Math.PI - ((2.0 * Math.PI * Y) / Math.Pow(2.0, zoom));

            lon = (float)((X / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            lat = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));
        }

        static void AdjustToTile(int zoom,ref double top, ref double left, ref double right, ref double bottom)
        {
            double X, Y;
            WorldToTilePos(left, top, zoom, out X, out Y);
            X = Math.Floor(X); Y = Math.Floor(Y);
            TileToWorldPos(X, Y, zoom, out left, out top);
            double X2, Y2;
            WorldToTilePos(right, bottom, zoom, out X2, out Y2);
            X2 = Math.Max(Math.Ceiling(X2),X+2);
            Y2 = Math.Max(Math.Ceiling(Y2),Y+2);
            TileToWorldPos(X2, Y2, zoom, out right, out bottom);
        }

        public MapDrawer(double top, double left, double right, double bottom)
        {
            var delta = Math.Max(Math.Abs(top - bottom), Math.Abs(right - left));

            _zoom = 0;
            while (_zoom <17 && (360 / (double)(1 << _zoom)) * 2 > delta) ++_zoom;
            _zoom = Math.Max(0, _zoom - 1);
            _top=Math.Max(top,bottom);
            _left=Math.Min(left,right);
            _right = Math.Max(left, right);
            _bottom = Math.Min(top,bottom);

            AdjustToTile(_zoom, ref _top, ref _left, ref _right, ref _bottom);

            var webText = new StringBuilder(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html><head><title>Map</title></head><body style=""margin: 0"">");

            double X, Y;
            WorldToTilePos(_left, _top, _zoom, out X, out Y);

            _lftTile = (int)X;
            _topTile = (int)Y;
            WorldToTilePos(_right, _bottom, _zoom, out X, out Y);
            var rgtTile = (int)X;
            var botTile = (int)Y;

            _pxWidth    =   Math.Max((rgtTile-_lftTile)*256,256);
            _pxHeight = Math.Max((botTile - _topTile) * 256,256);

            for(var i=_lftTile;i<rgtTile;++i)
                for (var j = _topTile; j < botTile; ++j)
                webText.AppendFormat(
                    @"<img src=""http://b.tile.openstreetmap.org/{0}/{1}/{2}.png"" style=""position:absolute; top:{3}px; left:{4}px;""/>",
                    _zoom, i, j, (j - _topTile) * 256, (i - _lftTile) * 256);

            webText.Append(@"<style>v\: * { behavior:url(#default#VML);display:inline-block }</style>");
            webText.AppendLine(@"<xml:namespace ns=""urn:schemas-microsoft-com:vml"" prefix=""v"" />");

            _basePage = webText.ToString();

            _vectorGraphics = new StringBuilder();
            Color = "red";
            Width = "2pt";
        }

        public MapDrawer(double lon, double lat,int zoom) :
            this(lat + (360 / (double)(1 << zoom)) * 2, lon - (360 / (double)(1 << zoom)) * 2,
                 lon + (360 / (double)(1 << zoom)) * 2, lat - (360 / (double)(1 << zoom)) * 2)
        {}

        public string ActiveHTML
        {
            get
            {
                var tmp = new StringBuilder(_basePage.Length + _vectorGraphics.Length + 80);

                tmp.Append(_basePage);
                tmp.Append(_vectorGraphics);
                tmp.Append("</body></html>");

                return tmp.ToString();
            }
        }

        void Coor2Point(double lon, double lat, out int x, out int y)
        {
            double X, Y;
            WorldToTilePos(lon, lat, _zoom, out X, out Y);

            x = (int)((X - _lftTile) * 256);
            y = (int)((Y - _topTile) * 256);
        }

        public string Color { get; set; }
        public string Width { get; set; }

        public void AppendMark(double lon, double lat)
        {
            int x, y;
            Coor2Point(lon, lat, out x, out y);
            _vectorGraphics.AppendFormat(@"<v:line style='position:absolute;left:{0}px;top:{1}px;' from='0px,0px' to='16px,16px' strokecolor='{2}' strokeweight='2pt'/>",
                x-8,y-8,Color);
            _vectorGraphics.AppendFormat(@"<v:line style='position:absolute;left:{0}px;top:{1}px;' from='16px,0px' to='0px,16px' strokecolor='{2}' strokeweight='2pt'/>",
                x - 8, y - 8, Color);
        }

        public void AppendSpot(double lon, double lat)
        {
            int x, y;
            Coor2Point(lon, lat, out x, out y);
            if (x < 0 || y < 0 || x > _pxWidth || y > _pxHeight) return;
            _vectorGraphics.AppendFormat(@"<v:rect style='position:absolute;left:{0}px;top:{1}px;' coordorigin='0px,0px' coordsize='4px,4px' strokecolor='{2}' fill='false'/>",
                x - 2, y - 2, Color);
        }

        enum OutCode
        {
            INSIDE = 0, // 0000
            LEFT = 1,   // 0001
            RIGHT = 2,  // 0010
            BOTTOM = 4, // 0100
            TOP = 8    // 1000
        };

        OutCode ComputeOutCode(int x, int y)
        {
            OutCode code = OutCode.INSIDE; // initialised as being inside of clip window
            if (x < 0)              code |= OutCode.LEFT;
            else if (x > _pxWidth)  code |= OutCode.RIGHT;
            if (y < 0)              code |= OutCode.BOTTOM;
            else if (y > _pxHeight) code |= OutCode.TOP;
            return code;
        }


        public void AppendLine(double lon1, double lat1, double lon2, double lat2)
        {
            int x0, y0, x1, y1;
            Coor2Point(lon1, lat1, out x0, out y0);
            Coor2Point(lon2, lat2, out x1, out y1);


            var outcode0 = ComputeOutCode(x0, y0);
            var outcode1 = ComputeOutCode(x1, y1);
            var accept = false;
            while (true)
            {
                if ((outcode0 | outcode1)==0)
                { // Bitwise OR is 0. Trivially accept and get out of loop
                    accept = true;
                    break;
                }
                else if ((outcode0 & outcode1)!=0)
                { // Bitwise AND is not 0. Trivially reject and get out of loop
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    int x=0, y=0;

                    // At least one endpoint is outside the clip rectangle; pick it.
                    OutCode outcodeOut = outcode0!=0 ? outcode0 : outcode1;

                    // Now find the intersection point;
                    // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                    if ((outcodeOut & OutCode.TOP)!=0)
                    {           // point is above the clip rectangle
                        x = x0 + (x1 - x0) * (_pxHeight - y0) / (y1 - y0);
                        y = _pxHeight;
                    }
                    else if ((outcodeOut & OutCode.BOTTOM)!=0)
                    { // point is below the clip rectangle
                        x = x0 + (x1 - x0) * (0 - y0) / (y1 - y0);
                        y = 0;
                    }
                    else if ((outcodeOut & OutCode.RIGHT)!=0)
                    {  // point is to the right of clip rectangle
                        y = y0 + (y1 - y0) * (_pxWidth - x0) / (x1 - x0);
                        x = _pxWidth;
                    }
                    else if ((outcodeOut & OutCode.LEFT) != 0)
                    {   // point is to the left of clip rectangle
                        y = y0 + (y1 - y0) * (0 - x0) / (x1 - x0);
                        x = 0;
                    }
                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if (outcodeOut == outcode0)
                    {
                        x0 = x;
                        y0 = y;
                        outcode0 = ComputeOutCode(x0, y0);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        outcode1 = ComputeOutCode(x1, y1);
                    }
                }
            }

            if (accept)
            {
                _vectorGraphics.AppendFormat(@"<v:line style='position:absolute;left:0px;top:0px;' from='{0}px,{1}px' to='{2}px,{3}x' strokecolor='{4}' strokeweight='3pt'/>",
                    x0, y0, x1, y1, Color);
            }
        }

    }
}
