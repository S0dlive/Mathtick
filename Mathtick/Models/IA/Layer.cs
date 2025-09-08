using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{

    public class Layer
    {
        public string Name { get; set; }
        public bool Visible { get; set; } = true;
        public List<string> NodeIds { get; set; } = new();

        public Layer(string name)
        {
            Name = name;
        }
    }
}
