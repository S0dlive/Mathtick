using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class AddNode : NodeBase
    {
        public AddNode()
        {
            Title = "Add";
            AddInput("a", "Number");
            AddInput("b", "Number");
            AddOutput("out", "Number");
        }

        public override object? Compute(Dictionary<string, object?> inputValues)
        {
            double a = Convert.ToDouble(inputValues["a"]);
            double b = Convert.ToDouble(inputValues["b"]);
            return a + b;
        }
    }
}
