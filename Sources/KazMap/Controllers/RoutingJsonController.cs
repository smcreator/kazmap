using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KazMap.Routing;
using KazMap.DataAccess;

namespace KazMap.Controllers
{
    public class RoutingJsonController : Controller
    {
        private RoutingModel _routing;

        public RoutingJsonController()
        {
            _routing = new RoutingModel();
        }

        public JsonResult Route(long startId, double latitude2, double longitude2)
        {
            var start = KazMapConnection.Read.Intersections.First(i => i.Id == startId);
            var end = _routing.GetIntersection(longitude2, latitude2);

            if (end == null)
                return Json(null);

            var route = _routing.FindRoute(start, end);
            return Json(route.Select(n => new { 
                Latitude1 = n.LeftNode.Latitude, 
                Longitude1 = n.LeftNode.Longitude,
                Latitude2 = n.RightNode.Latitude,
                Longitude2 = n.RightNode.Longitude
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
