using Mathtick.Models;
using Mathtick.Utils;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NCalc.Domain;
using Parlot.Fluent;
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
        private float scale = 40f;       
        private float offsetX = 0f;      
        private float offsetY = 0f;      
        private bool isDragging = false;
        private Point lastPointerPos;
        private List<MathExpression> Expressions = new List<MathExpression>();
        private string selectedFunction = null;

        public FunctionPage()
        {
            this.InitializeComponent();
            LoadFunctions();
            ExpressionList.ItemsSource = Expressions;
            
        }

        private void LoadFunctions()
        {
            foreach (var func in MathFunctionLibrary.Functions)
            {
                var item = new MenuFlyoutItem { Text = func.Name };
                item.Click += (s, e) =>
                {
                    string insertText = func.Name + "()";

                    int cursorPos = EquationBox.SelectionStart;

                    EquationBox.Text = EquationBox.Text.Insert(cursorPos, insertText);
                    EquationBox.SelectionStart = cursorPos + func.Name.Length + 1;
                    EquationBox.Focus(FocusState.Keyboard);
                    FunctionDropDown.Content = "Fonctions";
                };
                FunctionMenu.Items.Add(item);
            }
        }

        private void OnAddFunctionClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFunction) && string.IsNullOrWhiteSpace(EquationBox.Text)) return;

            string expr = string.IsNullOrEmpty(selectedFunction) ? EquationBox.Text : $"{selectedFunction}(x)";

            string name = $"f{Expressions.Count + 1}";

            Expressions.Add(new MathExpression { Name = name, Expression = expr, Color = Colors.Blue, IsVisible = true });

            ExpressionList.ItemsSource = null;
            ExpressionList.ItemsSource = Expressions;

            EquationBox.Text = "";
            selectedFunction = null;
            FunctionDropDown.Content = "Fonctions";

            GraphCanvas.Invalidate();
        }


        private void OnPlotClick(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Invalidate();
        }

        private void CanvasControl_Draw(CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            float width = (float)sender.ActualWidth;
            float height = (float)sender.ActualHeight;
            float centerX = width / 2 + offsetX;
            float centerY = height / 2 + offsetY;

            var textFormat = new CanvasTextFormat()
            {
                FontSize = 12,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            ds.Clear(Colors.White);

            ds.DrawLine(0, centerY, width, centerY, Colors.Black, 2f);
            ds.DrawLine(centerX, 0, centerX, height, Colors.Black, 2f);

            double[] steps = new double[] { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 };
            var unitStep = 1;

            for (int i = 0; i < steps.Length; i++)
            {
                if (steps[i] * scale >= 50)
                {
                    unitStep = (int)steps[i];
                    break;
                }
            }

            if (scale * unitStep < 50)
            {
                unitStep = (int)(50 / scale);

                int magnitude = (int)Math.Pow(10, Math.Floor(Math.Log10(unitStep)));
                int residual = unitStep / magnitude;
                if (residual <= 1) unitStep = 1 * magnitude;
                else if (residual <= 2) unitStep = 2 * magnitude;
                else if (residual <= 5) unitStep = 5 * magnitude;
                else unitStep = 10 * magnitude;
            }


            for (float px = centerX % scale; px < width; px += scale)
            {
                double xValue = (px - centerX) / scale;

                if (Math.Round(xValue) % unitStep == 0)
                {
                    ds.DrawLine(px, 0, px, height, Colors.LightGray, 1.5f);
                    ds.DrawText(((int)xValue).ToString(), px + 2, centerY + 5, Colors.Black, textFormat);
                }
            }

            for (float py = centerY % scale; py < height; py += scale)
            {
                double yValue = (centerY - py) / scale;

                if (Math.Round(yValue) % unitStep == 0)
                {
                    ds.DrawLine(0, py, width, py, Colors.LightGray, 1.5f);
                    ds.DrawText(((int)yValue).ToString(), centerX + 5, py - 10, Colors.Black, textFormat);
                }
            }

            foreach (var expr in Expressions)
            {
                if (string.IsNullOrWhiteSpace(expr.Expression) || !expr.IsVisible) continue;

                Func<double, double> f = FunctionParser.Parse(expr.Expression);
                if (f == null) continue;

                float? lastPx = null;
                float? lastPy = null;

                float step = Math.Max(1f, 1f / scale * 10f); 

                for (float px = 0; px < width; px += step)
                {
                    double x = (px - centerX) / scale;
                    double y;
                    try { y = f(x); }
                    catch { y = double.NaN; }

                    if (double.IsNaN(y) || double.IsInfinity(y))
                    {
                        lastPx = lastPy = null;
                        continue;
                    }

                    float py = (float)(centerY - y * scale);

                    if (lastPx != null && lastPy != null)
                        ds.DrawLine(lastPx.Value, lastPy.Value, px, py, expr.Color, 2f);

                    lastPx = px;
                    lastPy = py;
                }
            }
        }

        private void OnDeleteExpressionClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MathExpression expr)
            {
                Expressions.Remove(expr);
                ExpressionList.ItemsSource = null;
                ExpressionList.ItemsSource = Expressions;
                GraphCanvas.Invalidate();
            }
        }

        private void PredefinedColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button colorButton)
            { 
                if (colorButton.Background is SolidColorBrush brush)
                {
                    if (colorButton.DataContext is MathExpression expr)
                    {
                        expr.Color = brush.Color;

                        var container = ExpressionList.ContainerFromItem(expr) as ListViewItem;
                        if (container != null)
                        {
                            var split = FindVisualChild<SplitButton>(container);
                            if (split != null)
                            {
                                var border = split.Content as Border;
                                if (border != null)
                                    border.Background = brush;
                            }
                        }

                        GraphCanvas.Invalidate();
                    }
                }
            }
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            int count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T tChild)
                    return tChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void GraphCanvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var delta = e.GetCurrentPoint(GraphCanvas).Properties.MouseWheelDelta;
            if (delta == 0) return;

            float zoomFactor = (delta > 0) ? 1.1f : 0.9f;

            var pos = e.GetCurrentPoint(GraphCanvas).Position;
            float mouseX = (float)pos.X;
            float mouseY = (float)pos.Y;

            offsetX = mouseX - (mouseX - offsetX) * zoomFactor;
            offsetY = mouseY - (mouseY - offsetY) * zoomFactor;

            scale *= zoomFactor;
            GraphCanvas.Invalidate();
        }

        private void GraphCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDragging = true;
            lastPointerPos = e.GetCurrentPoint(GraphCanvas).Position;
            GraphCanvas.CapturePointer(e.Pointer);
        }

        private void GraphCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isDragging) return;

            var pos = e.GetCurrentPoint(GraphCanvas).Position;
            offsetX += (float)(pos.X - lastPointerPos.X);
            offsetY += (float)(pos.Y - lastPointerPos.Y);
            lastPointerPos = pos;

            GraphCanvas.Invalidate();
        }

        private void GraphCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDragging = false;
            GraphCanvas.ReleasePointerCapture(e.Pointer);
        }
    }
}
