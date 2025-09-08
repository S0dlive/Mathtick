using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class ConstantNode : NodeBase
    {
        public ConstantNode(double v = 0.0)
        {
            Title = "Constant";
            Params["Value"] = v;
            AddOutput("out", "Tensor");
        }
        public override object? Compute(Dictionary<string, object?> inputValues) => Params["Value"];
    }
}
