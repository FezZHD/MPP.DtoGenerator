using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DtoGenerator.DescriptionTypes;
using Newtonsoft.Json.Linq;

namespace DtoGenerator
{
    public class DtoGenarator
    {

        public DtoGenarator(string path)
        {
            StartJob(path);
        }

        private void StartJob(string path)
        {
            var classList = DeserialiazeJson<List<ClassDescription>>(ReadJsonFromFile(path));
        }


        private T DeserialiazeJson<T>(string json) where T : IEnumerable, new()
        {
            JToken jToken = JObject.Parse(json);
            var classList = jToken["classDescriptions"].ToObject<T>();
            return classList;;
        }


        private string ReadJsonFromFile(string path)
        {
            string resultString;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                resultString = streamReader.ReadToEnd();
            }
            return resultString;
        }
    }
}
