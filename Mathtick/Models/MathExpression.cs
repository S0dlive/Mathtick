using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Mathtick.Models
{
    public class MathExpression
    {
        public string Name { get; set; } = "f";
        public string Expression { get; set; } = "";
        public Color Color { get; set; } = Colors.Blue;
        public bool IsVisible { get; set; } = true;

        public string Display => $"{Name}(x) = {Expression}";
    }
}
