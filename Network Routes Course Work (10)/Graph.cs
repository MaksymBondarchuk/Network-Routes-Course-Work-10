using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Network_Routes_Course_Work_10
{
    public class Graph
    {
        public List<List<int>> Matrix { get; set; } = new List<List<int>>();
        
        public void LoadFromJson(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                var json = file.ReadToEnd();
                var deserializer = new JavaScriptSerializer();
                var result = deserializer.Deserialize<Graph>(json);
                Matrix = result.Matrix;
            }
        }

        public void LoadToJson(string fileName)
        {
            var json = new JavaScriptSerializer().Serialize(this);
            using (var file = new StreamWriter(fileName))
                file.WriteLine(json);
        }
    }
}
