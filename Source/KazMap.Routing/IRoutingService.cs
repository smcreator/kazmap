using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KazMap.Data;

namespace KazMap.Routing
{
    /// <summary>
    /// Сервис получения оптимального маршрута
    /// </summary>
    public interface IRoutingService
    {
        /// <summary>
        /// Получить маршрут по началу и концу
        /// </summary>
        /// <param name="startIntersection"></param>
        /// <param name="endIntersection"></param>
        /// <returns></returns>
        Route GetRoute(Intersections startIntersection, Intersections endIntersection);
        Intersections GetIntersection(double longitude, double latitude);
    }
}
