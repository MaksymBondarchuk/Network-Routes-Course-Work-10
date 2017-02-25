using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Network_Routes_Course_Work_10
{
    public class Path
    {
        public int Weight { get; set; }

        public List<int> Vertices { get; set; } = new List<int>();

        public static bool operator >(Path x, Path y)
        {
            return x.Weight > y.Weight;
        }

        public static bool operator <(Path x, Path y)
        {
            return x.Weight < y.Weight;
        }
        public static int operator +(Path x, Path y)
        {
            return x.Weight + y.Weight;
        }

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
            return result.Substring(0, result.Length - 1);
        }
    }
}
