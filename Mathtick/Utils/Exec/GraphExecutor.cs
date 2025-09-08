using Mathtick.Models.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathtick.Utils.Exec
{
    public sealed class GraphExecutor
    {
        private readonly GraphModel _graph;
        public GraphExecutor(GraphModel graph) => _graph = graph;

        public async Task<Dictionary<string, object?>> RunAsync(params string[] targetNodeIds)
            => await Task.Run(() => RunInternal(targetNodeIds));

        private Dictionary<string, object?> RunInternal(string[] targets)
        {
            var order = TopologicalOrder(); 
            var valueCache = new Dictionary<string, object?>(); 
            var portCache = new Dictionary<(string nodeId, string portName), object?>();

            foreach (var node in order)
            {

                var inputs = new Dictionary<string, object?>();
                foreach (var pIn in node.Inputs)
                {
                    var edge = _graph.Edges.FirstOrDefault(e => e.ToNodeId == node.Id && e.ToPortId == pIn.Id);
                    if (edge == null) continue; 
                    var fromNode = _graph.GetNode(edge.FromNodeId)!;
                    var fromPort = fromNode.Outputs.First(po => po.Id == edge.FromPortId);
                    var key = (fromNode.Id, fromPort.Name);
                    if (portCache.TryGetValue(key, out var v)) inputs[pIn.Name] = v;
                }

                var result = node.Compute(inputs);

                if (node.Outputs.Count <= 1)
                {
                    valueCache[node.Id] = result;
                    if (node.Outputs.Count == 1)
                        portCache[(node.Id, node.Outputs[0].Name)] = result;
                }
                else
                {
                    if (result is Dictionary<string, object?> rd)
                    {
                        foreach (var kv in rd)
                            portCache[(node.Id, kv.Key)] = kv.Value;
                    }
                    valueCache[node.Id] = result;
                }
            }

            var outDict = new Dictionary<string, object?>();
            foreach (var id in targets)
                outDict[id] = valueCache.TryGetValue(id, out var v) ? v : null;

            return outDict;
        }

        private List<NodeBase> TopologicalOrder()
        {
            var indeg = _graph.Nodes.ToDictionary(n => n.Id, _ => 0);
            foreach (var e in _graph.Edges)
            {
                if (indeg.ContainsKey(e.ToNodeId)) indeg[e.ToNodeId]++;
            }

            var q = new Queue<NodeBase>(_graph.Nodes.Where(n => indeg[n.Id] == 0));
            var order = new List<NodeBase>();

            while (q.Count > 0)
            {
                var n = q.Dequeue();
                order.Add(n);
                foreach (var e in _graph.Edges.Where(x => x.FromNodeId == n.Id))
                {
                    indeg[e.ToNodeId]--;
                    if (indeg[e.ToNodeId] == 0)
                        q.Enqueue(_graph.GetNode(e.ToNodeId)!);
                }
            }
            if (order.Count != _graph.Nodes.Count)
                throw new InvalidOperationException("Graph contains a cycle");

            return order;
        }
    }
}
