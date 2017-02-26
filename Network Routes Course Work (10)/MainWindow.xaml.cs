using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private Brush PathBrush { get; } = Brushes.Red;

        public MainWindow()
        {
            InitializeComponent();

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
            var conn = new Line
            {
                X1 = Graph.Vertices[idx1].Location.X,
                Y1 = Graph.Vertices[idx1].Location.Y,
                X2 = Graph.Vertices[idx2].Location.X,
                Y2 = Graph.Vertices[idx2].Location.Y,
                StrokeThickness = 2,
                Stroke = stroke
            };

            CanvasMain.Children.Add(conn);
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
            var grid = (Grid)CanvasMain.Children[Graph.Vertices[_onNode].CanvasIdx];
            grid.Margin = new Thickness(mousePos.X - grid.Height * .5, mousePos.Y - grid.Width * .5, 0, 0);

            // Moving node connections
            foreach (var conn in Graph.Vertices[_onNode].ConnectedBy)
                if (Graph.Vertices[_onNode].Location == Graph.Edges[conn].P1)
                {
                    Graph.Edges[conn].P1 = mousePos;

                    var line = (Line)CanvasMain.Children[Graph.Edges[conn].CanvasIdx];
                    line.X1 = mousePos.X;
                    line.Y1 = mousePos.Y;
                }
                else if (Graph.Vertices[_onNode].Location == Graph.Edges[conn].P2)
                {
                    Graph.Edges[conn].P2 = mousePos;

                    var line = (Line)CanvasMain.Children[Graph.Edges[conn].CanvasIdx];
                    line.X2 = mousePos.X;
                    line.Y2 = mousePos.Y;
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

        private void ListViewPathes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listViewItem = ListViewPathes.SelectedItem as ListViewItem;
            if (listViewItem == null)
                return;
            var path = Graph.Pathes[Convert.ToInt32(listViewItem.ColumnFrom)][Convert.ToInt32(listViewItem.ColumnTo)];

            foreach (var edge in Graph.Edges)
            {
                var line = CanvasMain.Children[edge.CanvasIdx] as Line;
                if (line == null) continue;
                line.Stroke = StrokeBrush;
                line.StrokeThickness = 2;
            }

            for (var i = 0; i < path.Vertices.Count - 1; i++)
            {
                var v1 = path.Vertices[i];
                var v2 = path.Vertices[i + 1];
                foreach (var edge in Graph.Edges)
                {
                    if (edge.Vertex1 != v1 && edge.Vertex2 != v1 || edge.Vertex2 != v2 && edge.Vertex1 != v2) continue;
                    var line = CanvasMain.Children[edge.CanvasIdx] as Line;
                    if (line == null) continue;
                    line.Stroke = PathBrush;
                    line.StrokeThickness = 4;
                }
            }
        }
    }
}
