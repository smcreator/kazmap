using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapRoutingDemo.ShortPath
{
    public interface IDijkstraSource
    {
        /// <summary>
        /// Calculate the Heuristic score and returns the point actual location
        /// </summary>
        /// <param name="id">Point ID</param>
        /// <param name="lon">Longitude</param>
        /// <param name="lat">Latitude</param>
        /// <returns>H Score</returns>
        int CalcHForNode(long id, out double lon, out double lat);

        /// <summary>
        /// Returns all edges from a point
        /// </summary>
        /// <param name="id">Point Id</param>
        /// <returns>List of edge (couple ID/Weight)</returns>
        IList<KeyValuePair<long, int>> GetLinks(long id);

        /// <summary>
        /// Indicates the search direction
        /// </summary>
        bool Direction { get; }
    }
}
