using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Network_Routes_Course_Work_10
{
    public partial class MainWindow
    {
        #region Properties
        // ReSharper disable once PossibleNullReferenceException
        private static Color FlashColor { get; } = (Color)ColorConverter.ConvertFromString("#FF007ACC");
        private Graph Graph { get; set; } = new Graph();
        /// <summary>
        /// Index of a vertex where pointer is (-1 for not on a node)
        /// </summary>
        private int MouseIsOnVertex { get; set; } = -1;
        private Brush StrokeBrush { get; } = Brushes.Black;
        private Brush FillBrush { get; } = Brushes.White;
        private Brush PathBrush { get; } = new SolidColorBrush(FlashColor);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            // For animation
            CanvasMain.Background = new SolidColorBrush(Colors.White);

            LoadAndDraw("..\\..\\Case\\Case 2.json");

            //try
            //{
            //    LoadAndDraw("..\\..\\Case\\Case 2.json");
            //}
            //catch (Exception)
            //{
            //    // ignored
            //}
        }

        #region File
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JavaScript Object Notation File|*.json"
            };

            if (dlg.ShowDialog() == true)
                LoadAndDraw(dlg.FileName);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = ".json",
                Filter = "JavaScript Object Notation File|*.json"
            };

            if (dlg.ShowDialog() == true)
                Graph.LoadToJson(dlg.FileName);
        }

        private void LoadAndDraw(string fileName)
        {
            Graph.LoadFromJson(fileName);
            LoadAndDrawWithoutFile();
        }
        private void LoadAndDrawWithoutFile()
        {
            DrawGraph();
            Graph.BruteForce();

            var size = Graph.Pathes.Count;
            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                    ListViewPathes.Items.Add(new ListViewItem
                    {
                        ColumnFrom = i.ToString(),
                        ColumnTo = j.ToString(),
                        ColumnPath = Graph.Pathes[i][j].ToString(),
                        ColumnWeight = Graph.Pathes[i][j].Weight.ToString()
                    });
        }
        #endregion

        #region Drawing
        private void DrawGraph()
        {
            var n = Graph.Vertices.Count;

            const double scale = 2.5;
            var r = Math.Min(CanvasMain.ActualWidth - Vertex.VertexSize * .5, CanvasMain.ActualHeight - Vertex.VertexSize * .5) / scale;
            var x0 = CanvasMain.ActualWidth / 2;
            var y0 = CanvasMain.ActualHeight / 2;

            for (var i = 0; i < Graph.Vertices.Count; i++)
            {
                var x = x0 + r * Math.Cos(Math.PI / 2 - 2 * Math.PI * i / n);
                var y = y0 + r * Math.Sin(Math.PI / 2 - 2 * Math.PI * i / n);

                AddNotVisual(new Point(x, y), i, StrokeBrush, FillBrush);
            }

            for (var i = 0; i < Graph.Vertices.Count; i++)
            {
                var i1 = i;
                foreach (var t in Graph.Vertices[i].ConnectedWith.Where(t => i1 < t))
                    ConnectNotVisual(i, t, StrokeBrush);
            }
        }

        private void AddNotVisual(Point pos, int idx, Brush stroke, Brush fill)
        {
            var grid = new Grid
            {
                Height = Vertex.VertexSize,
                Width = Vertex.VertexSize,
                Margin = new Thickness(pos.X - Vertex.VertexSize * .5, pos.Y - Vertex.VertexSize * .5, 0, 0)
            };

            var circle = new Ellipse
            {
                Height = Vertex.VertexSize,
                Width = Vertex.VertexSize,
                StrokeThickness = 2,
                Stroke = stroke,
                Fill = fill
            };
            grid.Children.Add(circle);

            var textBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = Vertex.VertexFontSize,
                Text = idx.ToString()
            };
            grid.Children.Add(textBlock);
            CanvasMain.Children.Add(grid);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 10);

            Graph.Vertices[idx].Location = pos;
            Graph.Vertices[idx].CanvasIdx = CanvasMain.Children.Count - 1;
        }

        private void ConnectNotVisual(int idx1, int idx2, Brush stroke)
        {
            var vertex1 = Graph.Vertices[idx1];
            var vertex2 = Graph.Vertices[idx2];
            var height = Math.Abs(vertex1.Location.Y - vertex2.Location.Y) + Edge.StrokeThickness;
            var width = Math.Abs(vertex1.Location.X - vertex2.Location.X) + Edge.StrokeThickness;
            var offsetX = Math.Min(vertex1.Location.X, vertex2.Location.X) - Edge.StrokeThickness * .5;
            var offsetY = Math.Min(vertex1.Location.Y, vertex2.Location.Y) - Edge.StrokeThickness * .5;
            var grid = new Grid
            {
                Height = height,
                Width = width,
                Margin = new Thickness(offsetX, offsetY, 0, 0)
            };

            var line = new Line
            {
                X1 = Graph.Vertices[idx1].Location.X - offsetX,
                Y1 = Graph.Vertices[idx1].Location.Y - offsetY,
                X2 = Graph.Vertices[idx2].Location.X - offsetX,
                Y2 = Graph.Vertices[idx2].Location.Y - offsetY,
                StrokeThickness = 2,
                Stroke = stroke
            };
            grid.Children.Add(line);

            var textBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = Edge.EdgeFontSize,
                Text = $" {Graph.Weights[idx1][idx2]} ",
                Background = Brushes.White,
            };
            grid.Children.Add(textBlock);

            CanvasMain.Children.Add(grid);
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 1);

            Graph.Edges.Add(new Edge
            {
                P1 = Graph.Vertices[idx1].Location,
                Vertex1 = idx1,
                P2 = Graph.Vertices[idx2].Location,
                Vertex2 = idx2,
                CanvasIdx = CanvasMain.Children.Count - 1
            });

            Graph.Vertices[idx1].ConnectedBy.Add(Graph.Edges.Count - 1);
            Graph.Vertices[idx2].ConnectedBy.Add(Graph.Edges.Count - 1);

        }

        private void FillVertex(int nodeIdx, Brush color)
        {
            var grid = (Grid)CanvasMain.Children[Graph.Vertices[nodeIdx].CanvasIdx];
            var nodeGray = (Ellipse)grid.Children[0];
            nodeGray.Fill = color;
        }

        /// <summary>
        /// Changes node location
        /// </summary>
        /// <param name="mousePos">New location</param>
        private void MoveVertex(Point mousePos)
        {
            // Moving node
            var gridVertex = (Grid)CanvasMain.Children[Graph.Vertices[MouseIsOnVertex].CanvasIdx];
            gridVertex.Margin = new Thickness(mousePos.X - gridVertex.Height * .5, mousePos.Y - gridVertex.Width * .5, 0, 0);

            // Moving node connections
            foreach (var e in Graph.Vertices[MouseIsOnVertex].ConnectedBy)
            {
                var edge = Graph.Edges[e];
                var vertex = Graph.Vertices[MouseIsOnVertex];
                if (vertex.Location == edge.P1)
                {
                    edge.P1 = mousePos;

                    var height = Math.Abs(vertex.Location.Y - edge.P2.Y) + Edge.StrokeThickness;
                    var width = Math.Abs(vertex.Location.X - edge.P2.X) + Edge.StrokeThickness;
                    var offsetX = Math.Min(vertex.Location.X, edge.P2.X) - Edge.StrokeThickness * .5;
                    var offsetY = Math.Min(vertex.Location.Y, edge.P2.Y) - Edge.StrokeThickness * .5;
                    var grid = (Grid)CanvasMain.Children[edge.CanvasIdx];
                    grid.Height = height;
                    grid.Width = width;
                    grid.Margin = new Thickness(offsetX, offsetY, 0, 0);

                    var line = (Line)grid.Children[0];
                    line.X1 = vertex.Location.X - offsetX;
                    line.Y1 = vertex.Location.Y - offsetY;
                    line.X2 = edge.P2.X - offsetX;
                    line.Y2 = edge.P2.Y - offsetY;
                }
                else if (vertex.Location == edge.P2)
                {
                    edge.P2 = mousePos;

                    var height = Math.Abs(vertex.Location.Y - edge.P1.Y) + Edge.StrokeThickness;
                    var width = Math.Abs(vertex.Location.X - edge.P1.X) + Edge.StrokeThickness;
                    var offsetX = Math.Min(vertex.Location.X, edge.P1.X) - Edge.StrokeThickness * .5;
                    var offsetY = Math.Min(vertex.Location.Y, edge.P1.Y) - Edge.StrokeThickness * .5;
                    var grid = (Grid)CanvasMain.Children[edge.CanvasIdx];
                    grid.Height = height;
                    grid.Width = width;
                    grid.Margin = new Thickness(offsetX, offsetY, 0, 0);

                    var line = (Line)grid.Children[0];
                    line.X2 = vertex.Location.X - offsetX;
                    line.Y2 = vertex.Location.Y - offsetY;
                    line.X1 = edge.P1.X - offsetX;
                    line.Y1 = edge.P1.Y - offsetY;
                }
            }

            Graph.Vertices[MouseIsOnVertex].Location = mousePos;
        }
        #endregion

        #region Mouse events
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            for (var i = 0; i < Graph.Vertices.Count; i++)
                if (Graph.Vertices[i].IsMyPoint(Mouse.GetPosition(CanvasMain)))
                {
                    MouseIsOnVertex = i;
                    break;
                }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseIsOnVertex = -1;
            foreach (var vertex in Graph.Vertices)
                Panel.SetZIndex(CanvasMain.Children[vertex.CanvasIdx], 10);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = Mouse.GetPosition(CanvasMain);

            if (Mouse.LeftButton == MouseButtonState.Pressed && MouseIsOnVertex != -1)
            {
                Panel.SetZIndex(CanvasMain.Children[Graph.Vertices[MouseIsOnVertex].CanvasIdx], 20);
                MoveVertex(mousePos);
            }
            else
                for (var i = 0; i < Graph.Vertices.Count; i++)
                    FillVertex(i, Graph.Vertices[i].IsMyPoint(mousePos) ? Brushes.Gray : FillBrush);
        }

        private void WindowMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasMain.Children.Clear();
            DrawGraph();
        }
        #endregion

        #region Pathes
        private async void ListViewPathes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listViewItem = ListViewPathes.SelectedItem as ListViewItem;
            if (listViewItem == null)
                return;
            var path = Graph.Pathes[Convert.ToInt32(listViewItem.ColumnFrom)][Convert.ToInt32(listViewItem.ColumnTo)];

            foreach (var edge in Graph.Edges)
            {
                var grid = (Grid)CanvasMain.Children[edge.CanvasIdx];
                var line = (Line)grid.Children[0];
                line.Stroke = StrokeBrush;
                line.StrokeThickness = 2;

                var textBlock = (TextBlock)grid.Children[1];
                textBlock.FontSize = Edge.EdgeFontSize;
                textBlock.Foreground = StrokeBrush;
                textBlock.FontWeight = FontWeights.Normal;
                Panel.SetZIndex(CanvasMain.Children[edge.CanvasIdx], 2);
            }

            for (var i = 0; i < path.Vertices.Count - 1; i++)
            {
                var v1 = path.Vertices[i];
                var v2 = path.Vertices[i + 1];
                foreach (var edge in Graph.Edges)
                {
                    if (edge.Vertex1 != v1 && edge.Vertex2 != v1 || edge.Vertex2 != v2 && edge.Vertex1 != v2) continue;
                    foreach (var child in CanvasMain.Children)
                        ((Grid)child).Visibility = Visibility.Hidden;
                    await CanvasFlash();
                    var grid = (Grid)CanvasMain.Children[edge.CanvasIdx];
                    var line = (Line)grid.Children[0];
                    line.Stroke = PathBrush;
                    line.StrokeThickness = 4;

                    var textBlock = (TextBlock)grid.Children[1];
                    textBlock.FontSize = Edge.EdgeFontSize * 2;
                    textBlock.Foreground = PathBrush;
                    textBlock.FontWeight = FontWeights.Bold;
                    Panel.SetZIndex(CanvasMain.Children[edge.CanvasIdx], 5);

                    CanvasUnFlash();
                    foreach (var child in CanvasMain.Children)
                        ((Grid)child).Visibility = Visibility.Visible;
                }
            }
        }

        private async Task CanvasFlash()
        {
            const int animationWait = 150;
            var cb = CanvasMain.Background;
            var da = new ColorAnimation
            {
                To = FlashColor,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationWait))
            };
            cb.BeginAnimation(SolidColorBrush.ColorProperty, da);
            await Task.Delay(animationWait);
        }

        private async void CanvasUnFlash()
        {
            const int animationWait = 150;
            var cb = CanvasMain.Background;
            var da = new ColorAnimation
            {
                To = Colors.White,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationWait))
            };
            cb.BeginAnimation(SolidColorBrush.ColorProperty, da);
            await Task.Delay(animationWait);
        }
        #endregion

        #region Add/Delete
        private void ButtonAddVertex_Click(object sender, RoutedEventArgs e)
        {
            CanvasMain.Children.Clear();
            ListViewPathes.Items.Clear();

            var weights = new List<List<int>>(Graph.Weights);
            Graph = new Graph
            {
                Weights = weights
            };

            Graph.Weights.Add(new List<int>());
            var newRow = Graph.Weights.Last();
            var weight = RadioButtonMin.IsChecked != null && (bool)RadioButtonMin.IsChecked 
                ? Graph.Weights.SelectMany(w => w).Where(w => w != 0).Min()
                : Graph.Weights.SelectMany(w => w).Where(w => w != 0).Max();
            for (var i = 0; i < Graph.Weights.Count - 1; i++)
            {
                Graph.Weights[i].Add(weight);
                newRow.Add(weight);
            }
            newRow.Add(0);
            Graph.LoadFromWeights();
            LoadAndDrawWithoutFile();
        }

        private void ButtonDeleteVertex_Click(object sender, RoutedEventArgs e)
        {
            CanvasMain.Children.Clear();
            ListViewPathes.Items.Clear();

            var weights = new List<List<int>>(Graph.Weights);
            Graph = new Graph
            {
                Weights = weights
            };

            var lastIndex = Graph.Weights.Count - 1;
            for (var i = 0; i < Graph.Weights.Count - 1; i++)
                Graph.Weights[i].RemoveAt(lastIndex);
            if (0 <= lastIndex)
                Graph.Weights.RemoveAt(lastIndex);

            Graph.LoadFromWeights();
            LoadAndDrawWithoutFile();
        }
        #endregion
    }
}