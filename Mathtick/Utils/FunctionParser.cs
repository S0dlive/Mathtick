using Mathtick.Models;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mathtick.Utils
{
    public class FunctionParser
    {
        public static Func<double, double> Parse(string expr)
        {
            expr = expr.Trim();

           
            expr = Regex.Replace(expr, @"(\d+(\.\d+)?|x)\s*\^\s*(\d+(\.\d+)?|x)", "Pow($1,$3)");

         

            return (x) =>
            {
                try
                {
                    var e = new Expression(expr);
                    e.Parameters["x"] = x;

                    e.EvaluateFunction += (name, args) =>
                    {
                        string func = name.ToLower();
                        var mathFunc = MathFunctionLibrary.Functions.Find(f => f.Name == func);
                        if (mathFunc != null)
                        {
                            if (args.Parameters.Length < mathFunc.ParameterCount)
                            {
                                args.Result = double.NaN;
                                return;
                            }

                            var paramValues = new double[mathFunc.ParameterCount];
                            for (int i = 0; i < mathFunc.ParameterCount; i++)
                                paramValues[i] = Convert.ToDouble(args.Parameters[i].Evaluate());

                            args.Result = mathFunc.Func(paramValues);
                        }
                    };
                    var result = e.Evaluate();
                    return Convert.ToDouble(result);
                }
                catch
                {
                    return double.NaN; 
                }
            };
        }
    }
}
