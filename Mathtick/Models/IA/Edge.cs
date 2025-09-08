using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Models.IA
{
    public sealed class Edge
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FromNodeId { get; set; } = "";
        public string FromPortId { get; set; } = "";
        public string ToNodeId { get; set; } = "";
        public string ToPortId { get; set; } = "";
    }
}
