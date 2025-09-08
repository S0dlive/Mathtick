using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public abstract class NodeBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "Node";

        public float X { get; set; }
        public float Y { get; set; }

        public List<Port> Inputs { get; } = new();
        public List<Port> Outputs { get; } = new();


        public Dictionary<string, object> Params { get; } = new();


        public abstract object? Compute(Dictionary<string, object?> inputValues);

        protected Port AddInput(string name, string type = "Tensor")
        {
            var p = new Port { Name = name, Direction = PortDirection.Input, NodeId = Id, DataType = type };
            Inputs.Add(p);
            return p;
        }
        protected Port AddOutput(string name, string type = "Tensor")
        {
            var p = new Port { Name = name, Direction = PortDirection.Output, NodeId = Id, DataType = type };
            Outputs.Add(p);
            return p;
        }
    }
}
