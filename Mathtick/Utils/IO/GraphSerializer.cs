using Mathtick.Models.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mathtick.Utils.IO
{
    public static class GraphSerializer
    {
        private static readonly JsonSerializerOptions _opt = new()
        {
            WriteIndented = true,
            Converters = { new NodePolymorphicConverter() }
        };

        public static string ToJson(GraphModel g) => JsonSerializer.Serialize(g, _opt);
        public static GraphModel FromJson(string json) => JsonSerializer.Deserialize<GraphModel>(json, _opt)!;
    }

    public sealed class NodePolymorphicConverter : JsonConverter<NodeBase>
    {
        public override NodeBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("Title", out var titleProp)) return null!;
            var title = titleProp.GetString() ?? "";

            NodeBase node = title switch
            {
                "Input" => new Mathtick.Models.IA.InputNode(),
                "Constant" => new Mathtick.Models.IA.ConstantNode(),
                "Add" => new Mathtick.Models.IA.AddNode(),
                "Multiply" => new Mathtick.Models.IA.MulNode(),
                "Sigmoid" => new Mathtick.Models.IA.SigmoidNode(),
                "Dense" => new Mathtick.Models.IA.DenseNode(),
                _ => throw new NotSupportedException($"Unknown node type: {title}")
            };

            var json = root.GetRawText();
            var clone = JsonSerializer.Deserialize<NodeClone>(json)!;

            node.Id = clone.Id;
            node.Title = clone.Title;
            node.X = clone.X; node.Y = clone.Y;

            node.Inputs.Clear(); node.Outputs.Clear();
            foreach (var p in clone.Inputs) node.Inputs.Add(p);
            foreach (var p in clone.Outputs) node.Outputs.Add(p);

            foreach (var kv in clone.Params)
                node.Params[kv.Key] = kv.Value!;

            return node;
        }

        public override void Write(Utf8JsonWriter writer, NodeBase value, JsonSerializerOptions options)
        {
            var clone = new NodeClone
            {
                Id = value.Id,
                Title = value.Title,
                X = value.X,
                Y = value.Y,
                Inputs = value.Inputs,
                Outputs = value.Outputs,
                Params = value.Params
            };
            JsonSerializer.Serialize(writer, clone, options);
        }

        private sealed class NodeClone
        {
            public string Id { get; set; } = "";
            public string Title { get; set; } = "";
            public float X { get; set; }
            public float Y { get; set; }
            public List<Mathtick.Models.IA.Port> Inputs { get; set; } = new();
            public List<Mathtick.Models.IA.Port> Outputs { get; set; } = new();
            public Dictionary<string, object?> Params { get; set; } = new();
        }
    }
}
