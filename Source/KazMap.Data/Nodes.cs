using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KazMap.Data
{
    [MetadataType(typeof(NodesMetadata))]
    public partial class Nodes
    {
        public string TagNames
        {
            get
            {
                return string.Join(",", Tags.Select(t => t.Value).ToArray());
            }
        }
    }

    public class NodesMetadata
    {
    }
}
