using System;
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
        private Graph Graph { get; } = new Graph();

        /// <summary>
        /// Index of a node where pointer is (-1 for not on a node)
        /// </summary>
        private int _onNode = -1;

        private Brush StrokeBrush { get; } = Brushes.Black;
        private Brush FillBrush { get; } = Brushes.White;
        //private Brush PathBrush { get; } = Brushes.Red;
        private Brush PathBrush { get; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));

        public MainWindow()
        {
            InitializeComponent();
            CanvasMain.Background = new SolidColorBrush(Colors.White);
            
            //DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            ////System.IO.Path.Combine(directoryInfo.FullName, "Case 2.json");
            //LoadAndDraw(System.IO.Path.Combine(directoryInfo.FullName, "Case 2.json"));
            LoadAndDraw("..\\..\\Case\\Case 2.json");
        }

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

        private void LoadAndDraw(string fileName)
        {
            Graph.LoadFromJson(fileName);
            DrawGraph();
            Graph.FloydWarshallWithPathReconstruction();

            var size = Graph.Pathes.Count;
            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                {
                    //var lwi = new ListViewItem
                    //{
                    //    ColumnFrom = i.ToString(),
                    //    ColumnTo = j.ToString(),
                    //    ColumnPath = Graph.Pathes[i][j].ToString(),
                    //    ColumnWeight = Graph.Pathes[i][j].Weight.ToString()
                    //};
                    ListViewPathes.Items.Add(new ListViewItem
                    {
                        ColumnFrom = i.ToString(),
                        ColumnTo = j.ToString(),
                        ColumnPath = Graph.Pathes[i][j].ToString(),
                        ColumnWeight = Graph.Pathes[i][j].Weight.ToString()
                    });
                }
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

        private void ButtonBuild_Click(object sender, RoutedEventArgs e)
        {
            Graph.FloydWarshallWithPathReconstruction();
        }

        /// <summary>
        /// Draws graph on canvas
        /// </summary>
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

            //ConnectNotVisual(0, 1, StrokeBrush);
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
            Panel.SetZIndex(CanvasMain.Children[CanvasMain.Children.Count - 1], 2);

            Graph.Vertices[idx].Location = pos;
            Graph.Vertices[idx].CanvasIdx = CanvasMain.Children.Count - 1;
        }

        private void ConnectNotVisual(int idx1, int idx2, Brush stroke)
        {
            var vertex1 = Graph.Vertices[idx1];
            var vertex2 = Graph.Vertices[idx2];

            const int strokeThickness = 30;
            const int halfStrokeThickness = 15;
            var height = Math.Abs(vertex1.Location.Y - vertex2.Location.Y) + strokeThickness;
            var width = Math.Abs(vertex1.Location.X - vertex2.Location.X) + strokeThickness;
            var offsetX = Math.Min(vertex1.Location.X, vertex2.Location.X) - halfStrokeThickness;
            var offsetY = Math.Min(vertex1.Location.Y, vertex2.Location.Y) - halfStrokeThickness;
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
                //Margin = new Thickness(40, 40, 0, 0),
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

            //Graph.Vertices[idx1].ConnectedWith.Add(idx2);
            Graph.Vertices[idx1].ConnectedBy.Add(Graph.Edges.Count - 1);
            //Graph.Vertices[idx2].ConnectedWith.Add(idx1);
            Graph.Vertices[idx2].ConnectedBy.Add(Graph.Edges.Count - 1);

        }

        /// <summary>
        /// Changes node location
        /// </summary>
        /// <param name="mousePos">New location</param>
        private void MoveNode(Point mousePos)
        {
            // Moving node
            var gridVertex = (Grid)CanvasMain.Children[Graph.Vertices[_onNode].CanvasIdx];
            gridVertex.Margin = new Thickness(mousePos.X - gridVertex.Height * .5, mousePos.Y - gridVertex.Width * .5, 0, 0);

            const int strokeThickness = 30;
            const int halfStrokeThickness = 15;

            // Moving node connections
            foreach (var e in Graph.Vertices[_onNode].ConnectedBy)
            {
                var edge = Graph.Edges[e];
                var vertex = Graph.Vertices[_onNode];
                if (vertex.Location == edge.P1)
                {
                    edge.P1 = mousePos;

                    var height = Math.Abs(vertex.Location.Y - edge.P2.Y) + strokeThickness;
                    var width = Math.Abs(vertex.Location.X - edge.P2.X) + strokeThickness;
                    var offsetX = Math.Min(vertex.Location.X, edge.P2.X) - halfStrokeThickness;
                    var offsetY = Math.Min(vertex.Location.Y, edge.P2.Y) - halfStrokeThickness;
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

                    var height = Math.Abs(vertex.Location.Y - edge.P1.Y) + strokeThickness;
                    var width = Math.Abs(vertex.Location.X - edge.P1.X) + strokeThickness;
                    var offsetX = Math.Min(vertex.Location.X, edge.P1.X) - halfStrokeThickness;
                    var offsetY = Math.Min(vertex.Location.Y, edge.P1.Y) - halfStrokeThickness;
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

            Graph.Vertices[_onNode].Location = mousePos;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            for (var i = 0; i < Graph.Vertices.Count; i++)
                if (Graph.Vertices[i].IsMyPoint(Mouse.GetPosition(CanvasMain)))
                {
                    _onNode = i;
                    break;
                }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _onNode = -1;
            foreach (var vertex in Graph.Vertices)
                Panel.SetZIndex(CanvasMain.Children[vertex.CanvasIdx], 2);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = Mouse.GetPosition(CanvasMain);

            if (Mouse.LeftButton == MouseButtonState.Pressed && _onNode != -1)
            {
                Panel.SetZIndex(CanvasMain.Children[Graph.Vertices[_onNode].CanvasIdx], 4);
                MoveNode(mousePos);

                //if (1 < Graph.Vertices.Select(v => v.IsMyPoint(mousePos)).Count())
                //{
                //    MoveNode(OldPoint);
                //    _onNode = -1;
                //    //Graph.Vertices[_onNode].Location = OldPoint;
                //}
            }
            else
            {
                for (var i = 0; i < Graph.Vertices.Count; i++)
                    FillNode(i, Graph.Vertices[i].IsMyPoint(mousePos) ? Brushes.Gray : FillBrush);
            }
        }

        private void FillNode(int nodeIdx, Brush color)
        {
            var grid = (Grid)CanvasMain.Children[Graph.Vertices[nodeIdx].CanvasIdx];
            var nodeGray = (Ellipse)grid.Children[0];
            nodeGray.Fill = color;
        }

        private void WindowMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasMain.Children.Clear();
            DrawGraph();
        }

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
            }

            for (var i = 0; i < path.Vertices.Count - 1; i++)
            {
                var v1 = path.Vertices[i];
                var v2 = path.Vertices[i + 1];
                foreach (var edge in Graph.Edges)
                {
                    if (edge.Vertex1 != v1 && edge.Vertex2 != v1 || edge.Vertex2 != v2 && edge.Vertex1 != v2) continue;
                    foreach (var child in CanvasMain.Children)
                        ((Grid) child).Visibility = Visibility.Hidden;
                    await CanvasFlash();
                    var grid = (Grid)CanvasMain.Children[edge.CanvasIdx];
                    var line = (Line)grid.Children[0];
                    line.Stroke = PathBrush;
                    line.StrokeThickness = 4;

                    var textBlock = (TextBlock)grid.Children[1];
                    textBlock.FontSize = Edge.EdgeFontSize * 2;
                    textBlock.Foreground = PathBrush;
                    textBlock.FontWeight = FontWeights.Bold;

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
            // ReSharper disable once PossibleNullReferenceException
            var convertFromString = (Color)ColorConverter.ConvertFromString("#FF007ACC");
            var da = new ColorAnimation
            {
                To = convertFromString,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationWait))
            };
            cb.BeginAnimation(SolidColorBrush.ColorProperty, da);
            await Task.Delay(animationWait);
        }

        private async Task CanvasUnFlash()
        {
            const int animationWait = 150;
            var cb = CanvasMain.Background;
            // ReSharper disable once PossibleNullReferenceException
            var da = new ColorAnimation
            {
                To = Colors.White,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationWait))
            };
            cb.BeginAnimation(SolidColorBrush.ColorProperty, da);
            await Task.Delay(animationWait);
        }
    }
}
