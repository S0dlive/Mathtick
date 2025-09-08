using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class DenseNode : NodeBase
    {
        public DenseNode(int inSize = 1, int outSize = 1)
        {
            Title = "Dense";
            AddInput("x", "Tensor");
            AddOutput("y", "Tensor");

            Params["W"] = CreateMatrix(outSize, inSize, 0.1);
            Params["b"] = CreateVector(outSize, 0.0);
            Params["Activation"] = "none"; 
        }

        public override object? Compute(Dictionary<string, object?> inputValues)
        {
            // x: double[] attendu
            var x = ToVector(inputValues["x"]);
            var W = (double[][])Params["W"];
            var b = (double[])Params["b"];
            int outSize = W.Length;
            int inSize = W[0].Length;

            if (x.Length != inSize) throw new InvalidOperationException("Dense: input size mismatch.");

            var y = new double[outSize];
            for (int i = 0; i < outSize; i++)
            {
                double sum = b[i];
                for (int j = 0; j < inSize; j++) sum += W[i][j] * x[j];
                y[i] = sum;
            }

            var act = (string)Params["Activation"];
            if (act == "sigmoid")
            {
                for (int i = 0; i < y.Length; i++) y[i] = 1.0 / (1.0 + Math.Exp(-y[i]));
            }

            return y;
        }

        private static double[][] CreateMatrix(int r, int c, double v)
        {
            var m = new double[r][];
            for (int i = 0; i < r; i++)
            {
                m[i] = new double[c];
                for (int j = 0; j < c; j++) m[i][j] = v;
            }
            return m;
        }
        private static double[] CreateVector(int n, double v) => Enumerable.Repeat(v, n).ToArray();

        private static double[] ToVector(object? o)
        {
            return o switch
            {
                double d => new[] { d },
                double[] arr => arr,
                float[] farr => farr.Select(f => (double)f).ToArray(),
                IEnumerable<double> en => en.ToArray(),
                _ => throw new InvalidOperationException("Dense: unsupported input tensor")
            };
        }
    }
}
