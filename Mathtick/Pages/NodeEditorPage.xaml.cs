using Mathtick.Models.IA;
using Mathtick.Utils.Exec;
using Mathtick.Utils.IO;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Mathtick.Pages;

public sealed partial class NodeEditorPage : Page
{
    private readonly GraphModel _graph = new();
    private readonly Dictionary<string, (float w, float h)> _nodeSize = new();
    private string? _dragNodeId;
    private Vector2 _dragOffset;
    private (string nodeId, string portId, bool isDragging)? _pendingLink;
    private CanvasSolidColorBrush? _nodeBrush, _portBrush, _textBrush, _edgeBrush;

    public NodeEditorPage()
    {
        this.InitializeComponent();

        if (!_graph.Layers.ContainsKey("Default"))
            _graph.Layers["Default"] = new Layer("Default");
    }

    private void NodeCanvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        _nodeBrush = new CanvasSolidColorBrush(sender, Colors.White);
        _portBrush = new CanvasSolidColorBrush(sender, Colors.DodgerBlue);
        _textBrush = new CanvasSolidColorBrush(sender, Colors.Black);
        _edgeBrush = new CanvasSolidColorBrush(sender, Colors.Gray);
    }

    #region UI Buttons

    private void AddNode(NodeBase n, float x = 80, float y = 80, string layerName = "Default")
    {
        n.X = x; n.Y = y;
        _graph.Nodes.Add(n);
        _nodeSize[n.Id] = (160, 80);

        if (!_graph.Layers.ContainsKey(layerName))
            _graph.Layers[layerName] = new Layer(layerName);

        _graph.Layers[layerName].NodeIds.Add(n.Id);
        NodeCanvas.Invalidate();
    }

    private void AddInput_Click(object sender, RoutedEventArgs e) => AddNode(new InputNode(), 80, 120);
    private void AddConst_Click(object sender, RoutedEventArgs e) => AddNode(new ConstantNode(2.0), 80, 240);
    private void AddAdd_Click(object sender, RoutedEventArgs e) => AddNode(new AddNode(), 360, 160);
    private void AddMul_Click(object sender, RoutedEventArgs e) => AddNode(new MulNode(), 360, 260);
    private void AddSig_Click(object sender, RoutedEventArgs e) => AddNode(new SigmoidNode(), 580, 200);
    private void AddDense_Click(object sender, RoutedEventArgs e) => AddNode(new DenseNode(1, 1), 580, 320);

    private async void Run_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var exec = new GraphExecutor(_graph);
            var targetIds = _graph.Nodes
                .Where(n => !_graph.Edges.Any(e2 => e2.FromNodeId == n.Id))
                .Select(n => n.Id).ToArray();

            var res = await exec.RunAsync(targetIds);
            var txt = string.Join("\n", res.Select(kv => $"{_graph.GetNode(kv.Key)!.Title} = {ValueToString(kv.Value)}"));
            await new ContentDialog
            {
                Title = "Résultats",
                Content = txt,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }
        catch (Exception ex)
        {
            await new ContentDialog
            {
                Title = "Erreur",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }
    }

    #endregion

    #region Drawing & Interaction

    private void NodeCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var ds = args.DrawingSession;

        ds.Clear(Colors.LightGray);


        foreach (var e in _graph.Edges)
        {
            var fromNode = _graph.GetNode(e.FromNodeId)!;
            var toNode = _graph.GetNode(e.ToNodeId)!;
            var fromPort = fromNode.Outputs.First(p => p.Id == e.FromPortId);
            var toPort = toNode.Inputs.First(p => p.Id == e.ToPortId);

            var p1 = GetPortCenter(fromNode, fromPort, _nodeSize[fromNode.Id]);
            var p2 = GetPortCenter(toNode, toPort, _nodeSize[toNode.Id]);
            DrawBezier(ds, p1, p2);
        }
        if (_pendingLink is { isDragging: true } pl)
        {
            var fromNode = _graph.GetNode(pl.nodeId)!;
            var fromPort = fromNode.Outputs.First(p => p.Id == pl.portId);
            var p1 = GetPortCenter(fromNode, fromPort, _nodeSize[fromNode.Id]);
            DrawBezier(ds, p1, _lastPointer);
        }

        foreach (var layer in _graph.Layers.Values)
        {
            if (!layer.Visible) continue;

            foreach (var nodeId in layer.NodeIds)
            {
                var n = _graph.GetNode(nodeId);
                if (n == null) continue;
                DrawNode(ds, n, _nodeSize[n.Id]);
            }
        }
    }

    private void DrawNode(CanvasDrawingSession ds, NodeBase n, (float w, float h) size)
    {
        var (w, h) = size;
        var rect = new Windows.Foundation.Rect(n.X, n.Y, w, h);
        ds.FillRoundedRectangle(rect, 12, 12, _nodeBrush!);
        ds.DrawRoundedRectangle(rect, 12, 12, Colors.Gray, 2f);


        ds.DrawText(n.Title, new Vector2(n.X + 10, n.Y + 8), _textBrush!.Color);


        for (int i = 0; i < n.Inputs.Count; i++)
        {
            var p = n.Inputs[i];
            var cy = n.Y + 30 + i * 20;
            ds.FillCircle(n.X, cy, 6, new CanvasSolidColorBrush(ds, Colors.MediumPurple));
            ds.DrawText(p.Name, new Vector2(n.X + 10, cy - 8), _textBrush!.Color);
        }

        for (int i = 0; i < n.Outputs.Count; i++)
        {
            var p = n.Outputs[i];
            var cy = n.Y + 30 + i * 20;
            ds.FillCircle(n.X + w, cy, 6, new CanvasSolidColorBrush(ds, Colors.LightSkyBlue));
            ds.DrawText(p.Name, new Vector2(n.X + w - 50, cy - 8), _textBrush!.Color);
        }
    }

    private Vector2 GetPortCenter(NodeBase n, Port p, (float w, float h) size)
    {
        var (w, h) = size;
        if (p.Direction == PortDirection.Input)
        {
            int idx = n.Inputs.FindIndex(x => x.Id == p.Id);
            return new Vector2(n.X, n.Y + 30 + idx * 20);
        }
        else
        {
            int idx = n.Outputs.FindIndex(x => x.Id == p.Id);
            return new Vector2(n.X + w, n.Y + 30 + idx * 20);
        }
    }

    private void DrawBezier(CanvasDrawingSession ds, Vector2 a, Vector2 b, Color? color = null, float thickness = 2f)
    {
        color ??= Colors.Gray;
        float dx = Math.Abs(b.X - a.X);
        var c1 = new Vector2(a.X + dx * 0.5f, a.Y);
        var c2 = new Vector2(b.X - dx * 0.5f, b.Y);

        using (var pb = new CanvasPathBuilder(ds))
        {
            pb.BeginFigure(a);
            pb.AddCubicBezier(c1, c2, b);
            pb.EndFigure(CanvasFigureLoop.Open);

            using (var geom = CanvasGeometry.CreatePath(pb))
            {
                ds.DrawGeometry(geom, color.Value, thickness);
            }
        }
    }

    private Vector2 _lastPointer;

    private void NodeCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(NodeCanvas).Position;
        _lastPointer = new Vector2((float)p.X, (float)p.Y);

        foreach (var n in _graph.Nodes)
        {
            foreach (var op in n.Outputs)
            {
                var c = GetPortCenter(n, op, _nodeSize[n.Id]);
                if (Vector2.Distance(c, _lastPointer) < 10f)
                {
                    _pendingLink = (n.Id, op.Id, true);
                    return;
                }
            }
        }

        foreach (var n in _graph.Nodes.AsEnumerable().Reverse())
        {
            var (w, h) = _nodeSize[n.Id];
            if (p.X >= n.X && p.X <= n.X + w && p.Y >= n.Y && p.Y <= n.Y + h)
            {
                _dragNodeId = n.Id;
                _dragOffset = new Vector2((float)p.X - n.X, (float)p.Y - n.Y);
                return;
            }
        }
    }

    private void NodeCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(NodeCanvas).Position;
        _lastPointer = new Vector2((float)p.X, (float)p.Y);

        if (_dragNodeId is string id)
        {
            var n = _graph.GetNode(id)!;
            n.X = (float)p.X - _dragOffset.X;
            n.Y = (float)p.Y - _dragOffset.Y;
            NodeCanvas.Invalidate();
        }
        else if (_pendingLink is { isDragging: true })
        {
            NodeCanvas.Invalidate();
        }
    }

    private void NodeCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var p = e.GetCurrentPoint(NodeCanvas).Position;
        var pos = new Vector2((float)p.X, (float)p.Y);

        if (_pendingLink is { isDragging: true } pl)
        {
            foreach (var n in _graph.Nodes)
            {
                foreach (var ip in n.Inputs)
                {
                    var c = GetPortCenter(n, ip, _nodeSize[n.Id]);
                    if (Vector2.Distance(c, pos) < 10f)
                    {
                        _graph.Edges.Add(new Edge
                        {
                            FromNodeId = pl.nodeId,
                            FromPortId = pl.portId,
                            ToNodeId = n.Id,
                            ToPortId = ip.Id
                        });
                        NodeCanvas.Invalidate();
                        _pendingLink = null;
                        return;
                    }
                }
            }
            _pendingLink = null;
        }

        _dragNodeId = null;
    }

    private static string ValueToString(object? v)
    {
        if (v is null) return "null";
        if (v is double d) return d.ToString("G6");
        if (v is double[] arr) return "[" + string.Join(", ", arr.Select(x => x.ToString("G4"))) + "]";
        return v.ToString() ?? "";
    }

    #endregion
}
