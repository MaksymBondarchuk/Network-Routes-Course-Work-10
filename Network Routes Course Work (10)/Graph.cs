using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Network_Routes_Course_Work_10
{
    public class Graph
    {
        public List<List<int>> Weights { get; set; } = new List<List<int>>();
        public List<List<Path>> Pathes { get; set; } = new List<List<Path>>();

        public void LoadFromJson(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                var json = file.ReadToEnd();
                var deserializer = new JavaScriptSerializer();
                var result = deserializer.Deserialize<Graph>(json);
                Weights = result.Weights;
            }
        }

        public void LoadToJson(string fileName)
        {
            var json = new JavaScriptSerializer().Serialize(this);
            using (var file = new StreamWriter(fileName))
                file.WriteLine(json);
        }

        public void BuildPathes()
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
    }
}
