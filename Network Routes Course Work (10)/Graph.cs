using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;

namespace Network_Routes_Course_Work_10
{
    public class Graph
    {
        #region Visual
        public readonly List<Vertex> Vertices = new List<Vertex>();
        public readonly List<Edge> Edges = new List<Edge>();
        #endregion


        #region Math
        // ReSharper disable once MemberCanBePrivate.Global
        public List<List<int>> Weights { get; set; } = new List<List<int>>();
        private List<List<int>> Next { get; } = new List<List<int>>();
        public List<List<Path>> Pathes { get; } = new List<List<Path>>();

        public void LoadFromJson(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                var json = file.ReadToEnd();
                var deserializer = new JavaScriptSerializer();
                var result = deserializer.Deserialize<Graph>(json);
                Weights = result.Weights;
            }

            LoadFromWeights();
        }

        public void LoadFromWeights()
        {
            var size = Weights.Count;
            for (var i = 0; i < size; i++)
                Vertices.Add(new Vertex());

            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                {
                    Vertices[i].ConnectedWith.Add(j);
                    Vertices[j].ConnectedWith.Add(i);
                }
        }

        public void LoadToJson(string fileName)
        {
            var json = new JavaScriptSerializer().Serialize(this);
            using (var file = new StreamWriter(fileName))
                file.WriteLine(json);
        }

        public void FloydWarshall()
        {
            Pathes.Clear();

            var size = Weights.Count;
            for (var i = 0; i < size; i++)
            {
                Pathes.Add(new List<Path>());
                for (var j = 0; j < size; j++)
                    Pathes[i].Add(new Path { Weight = Weights[i][j] });
            }

            for (var k = 0; k < size; k++)
                for (var i = 0; i < size; i++)
                    for (var j = 0; j < size; j++)
                        if (Pathes[i][j].Weight > Pathes[i][k].Weight + Pathes[k][j].Weight)
                            Pathes[i][j].Weight = Pathes[i][k].Weight + Pathes[k][j].Weight;
        }

        public void FloydWarshallWithPathReconstruction()
        {
            Pathes.Clear();
            Next.Clear();

            var size = Weights.Count;
            for (var u = 0; u < size; u++)
            {
                Pathes.Add(new List<Path>());
                Next.Add(new List<int>());
                for (var v = 0; v < size; v++)
                {
                    Pathes[u].Add(new Path { Weight = Weights[u][v] });
                    Next[u].Add(v);
                }
            }

            for (var k = 0; k < size; k++)
                for (var i = 0; i < size; i++)
                    for (var j = 0; j < size; j++)
                    {
                        //if (j == 7)
                        //{
                            
                        //}
                        if (Pathes[i][j].Weight > Pathes[i][k].Weight + Pathes[k][j].Weight)
                        {
                            Pathes[i][j].Weight = Pathes[i][k].Weight + Pathes[k][j].Weight;
                            Next[i][j] = Next[i][k];
                        }
                    }

            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                    Pathes[i][j].RestoreVertices(Next, i, j);
        }
        #endregion
    }
}
