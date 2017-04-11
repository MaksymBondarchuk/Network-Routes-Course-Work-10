using System.Collections.Generic;
using System.Linq;

namespace Network_Routes_Course_Work_10
{
    public class Path
    {
        public int Weight { get; set; }

        public List<int> Vertices { get; set; } = new List<int>();

        public override string ToString()
        {
            var result = Vertices.Aggregate(string.Empty, (current, vertex) => current + $"{vertex}, ");
            return result.Substring(0, result.Length - 2);
        }
    }
}
