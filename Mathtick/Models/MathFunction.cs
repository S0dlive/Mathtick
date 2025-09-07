using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models
{
    public class MathFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParameterCount { get; set; }
        public Func<double[], double> Func { get; set; }
    }

    public static class MathFunctionLibrary
    {
        public static List<MathFunction> Functions = new List<MathFunction>()
        {
            new MathFunction
            {
                Name = "sin",
                Description = "Sinus (radians)",
                ParameterCount = 1,
                Func = args => Math.Sin(args[0])
            },
            new MathFunction
            {
                Name = "cos",
                Description = "Cosinus (radians)",
                ParameterCount = 1,
                Func = args => Math.Cos(args[0])
            },
            new MathFunction
            {
                Name = "tan",
                Description = "Tangente (radians)",
                ParameterCount = 1,
                Func = args => Math.Tan(args[0])
            },
            new MathFunction
            {
                Name = "asin",
                Description = "Arc sinus",
                ParameterCount = 1,
                Func = args => Math.Asin(args[0])
            },
            new MathFunction
            {
                Name = "acos",
                Description = "Arc cosinus",
                ParameterCount = 1,
                Func = args => Math.Acos(args[0])
            },
            new MathFunction
            {
                Name = "atan",
                Description = "Arc tangente",
                ParameterCount = 1,
                Func = args => Math.Atan(args[0])
            },
            new MathFunction
            {
                Name = "sinh",
                Description = "Sinus hyperbolique",
                ParameterCount = 1,
                Func = args => Math.Sinh(args[0])
            },
            new MathFunction
            {
                Name = "cosh",
                Description = "Cosinus hyperbolique",
                ParameterCount = 1,
                Func = args => Math.Cosh(args[0])
            },
            new MathFunction
            {
                Name = "tanh",
                Description = "Tangente hyperbolique",
                ParameterCount = 1,
                Func = args => Math.Tanh(args[0])
            },
            new MathFunction
            {
                Name = "log",
                Description = "Logarithme base 10",
                ParameterCount = 1,
                Func = args => Math.Log10(args[0])
            },
            new MathFunction
            {
                Name = "ln",
                Description = "Logarithme népérien",
                ParameterCount = 1,
                Func = args => Math.Log(args[0])
            },
            new MathFunction
            {
                Name = "exp",
                Description = "Exponentielle e^x",
                ParameterCount = 1,
                Func = args => Math.Exp(args[0])
            },
            new MathFunction
            {
                Name = "pow",
                Description = "Puissance a^b",
                ParameterCount = 2,
                Func = args => Math.Pow(args[0], args[1])
            },
            new MathFunction
            {
                Name = "sqrt",
                Description = "Racine carrée",
                ParameterCount = 1,
                Func = args => Math.Sqrt(args[0])
            },
            new MathFunction
            {
                Name = "abs",
                Description = "Valeur absolue",
                ParameterCount = 1,
                Func = args => Math.Abs(args[0])
            },
            new MathFunction
            {
                Name = "floor",
                Description = "Arrondi inférieur",
                ParameterCount = 1,
                Func = args => Math.Floor(args[0])
            },
            new MathFunction
            {
                Name = "ceil",
                Description = "Arrondi supérieur",
                ParameterCount = 1,
                Func = args => Math.Ceiling(args[0])
            },
            new MathFunction
            {
                Name = "round",
                Description = "Arrondi au plus proche",
                ParameterCount = 1,
                Func = args => Math.Round(args[0])
            },
            new MathFunction
            {
                Name = "sigmoid",
                Description = "Sigmoid : 1 / (1 + e^-x)",
                ParameterCount = 1,
                Func = args => 1.0 / (1.0 + Math.Exp(-args[0]))
            }
        };
    }
}
