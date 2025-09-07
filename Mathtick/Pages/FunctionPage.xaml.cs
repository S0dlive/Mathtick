using Mathtick.Utils;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mathtick.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FunctionPage : Page
    {
        public FunctionPage()
        {
            InitializeComponent();
            GraphCanvas.Invalidate();
        }
        private List<Func<double, double>> functions = new List<Func<double, double>>();
        private void OnPlotClick(object sender, RoutedEventArgs e)
        {
            string expr = EquationBox.Text.Trim();

            functions.Add(FunctionParser.Parse(expr));

            GraphCanvas.Invalidate();
        }
        void CanvasControl_Draw(
        Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender,
        Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            float w = (float)sender.ActualWidth, h = (float)sender.ActualHeight;
            float cx = w / 2, cy = h / 2, scale = 40;

            ds.DrawLine(0, cy, w, cy, Colors.Gray);
            ds.DrawLine(cx, 0, cx, h, Colors.Gray);

            var colors = new[] { Colors.Red, Colors.Blue, Colors.Green, Colors.LightGray, Colors.Violet, Colors.Orange, Colors.OrangeRed, Colors.Purple };
            for (int i = 0; i < functions.Count; i++)
            {
                var f = functions[i];
                var col = colors[i % colors.Length];
                Vector2? prev = null;

                for (float px = 0; px < w; px++)
                {
                    double x = (px - cx) / scale;
                    double y = f(x);
                    float py = cy - (float)(y * scale);
                    var p = new Vector2(px, py);
                    if (prev.HasValue) ds.DrawLine(prev.Value, p, col, 2);
                    prev = p;
                }
            }
        }
    }
}
