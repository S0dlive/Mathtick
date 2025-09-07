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
