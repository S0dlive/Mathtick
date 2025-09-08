using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class SigmoidNode : NodeBase
    {
        public SigmoidNode()
        {
            Title = "Sigmoid";
            AddInput("x", "Number");
            AddOutput("out", "Number");
        }
        public override object? Compute(Dictionary<string, object?> inputValues)
        {
            double x = Convert.ToDouble(inputValues["x"]);
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}
