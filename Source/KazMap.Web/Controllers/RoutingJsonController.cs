using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KazMap.Routing;
using KazMap.Data;

namespace KazMap.Web.Controllers
{
    public class RoutingJsonController : Controller
    {
        private KazMapEntities _db = new KazMapEntities();
        private IRoutingService _routing = new RoutingService();

        public JsonResult Route(long startId, double latitude2, double longitude2)
        {
            var start = _db.Intersections.First(i => i.Id == startId);
            var end = _routing.GetIntersection(longitude2, latitude2);

            if (end == null)
                return Json(null);

            var route = _routing.GetRoute(start, end);
            return Json(route.Select(n => new { 
                Latitude1 = n.LeftNode.Latitude, 
                Longitude1 = n.LeftNode.Longitude,
                Latitude2 = n.RightNode.Latitude,
                Longitude2 = n.RightNode.Longitude
            }), JsonRequestBehavior.AllowGet);
        }

    }
}
