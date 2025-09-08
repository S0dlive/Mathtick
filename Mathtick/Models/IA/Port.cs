using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public enum PortDirection { Input, Output }

    public sealed class Port
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public PortDirection Direction { get; set; }
        public string DataType { get; set; } = "Tensor"; 
        public string NodeId { get; set; } = "";
    }
}
