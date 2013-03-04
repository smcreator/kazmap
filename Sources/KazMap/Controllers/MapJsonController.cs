using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KazMap.DataAccess;

namespace KazMap.Controllers
{ 
    public class MapJsonController : Controller
    {
        private KazMapConnection db = new KazMapConnection();

        public JsonResult Surface(double left, double right, double top, double bottom)
        {
            var nodes = db.Surface(new Bounds
                {
                    LeftTop = new Point { Longitude = left, Latitude = top },
                    RightBottom = new Point { Longitude = right, Latitude = bottom }
                }).Select(n => new {
                    Id = n.Id,
                    Longitude = n.Longitude,
                    Latitude = n.Latitude                 
                });
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }
    }
}