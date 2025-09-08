using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class GraphModel
    {
        public List<NodeBase> Nodes { get; set; } = new();
        public List<Edge> Edges { get; set; } = new();

        public Dictionary<string, Layer> Layers { get; set; } = new();
        public GraphModel()
        {
            Layers["Default"] = new Layer("Default");
        }
        public NodeBase? GetNode(string id) => Nodes.FirstOrDefault(n => n.Id == id);
        public Port? GetPort(string nodeId, string portId)
            => GetNode(nodeId)?.Inputs.Concat(GetNode(nodeId)!.Outputs).FirstOrDefault(p => p.Id == portId);
    }
}
