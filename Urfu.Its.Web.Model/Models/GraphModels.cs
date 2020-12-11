using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Urfu.Its.Web.Models
{
    public class GraphModel
    {
        public List<GraphNode> nodes { get; set; }
        public List<GraphEdge> edges { get; set; }
    }

    public class GraphEdge
    {
        public string id { get; set; }
        public string source { get; set; }
        public string target { get; set; }
    }

    public class GraphNode
    {
        public string id { get; set; }
        public string label { get; set; }
        public int? x { get; set; }
        public int? y { get; set; }
        public int? size { get; set; }
    }
}