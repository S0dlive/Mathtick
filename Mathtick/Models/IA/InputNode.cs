using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class InputNode : NodeBase
    {
        public InputNode()
        {
            Title = "Input";
            AddOutput("out", "Tensor");

            Params["Value"] = 0.0;
        }

        public override object? Compute(Dictionary<string, object?> inputValues)
            => Params["Value"];
    }
}
