using System.Collections.Generic;
using System.Linq;

namespace Network_Routes_Course_Work_10
{
    public class Path
    {
        public int Weight { get; set; }

        public List<int> Vertices { get; set; } = new List<int>();
        
        public void RestoreVertices(List<List<int>> next, int u, int v)
        {
            Vertices.Add(u);
            while (u != v)
            {
                u = next[u][v];
                Vertices.Add(u);
            }
        }

        public override string ToString()
        {
            var result = Vertices.Aggregate(string.Empty, (current, vertex) => current + $"{vertex}, ");
            return result.Substring(0, result.Length - 2);
        }
    }
}
