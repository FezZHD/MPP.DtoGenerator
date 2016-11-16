using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DefaultTypes;
using DtoGenerator.DescriptionTypes;
using Newtonsoft.Json.Linq;
using TypeInterface;

namespace DtoGenerator
{
    public class DtoGenarator
    {

        internal List<IType> TypeList = new List<IType>();

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


        private void LoadDefaultTypes()
        {
            TypeList.Add(new BooleanType());
            TypeList.Add(new ByteType());
            TypeList.Add(new DateType());
            TypeList.Add(new FloatType());
            TypeList.Add(new Integer32Type());
            TypeList.Add(new Integer64Type());
            TypeList.Add(new StringType());
            //todo make assembly loader
        }
    }
}
