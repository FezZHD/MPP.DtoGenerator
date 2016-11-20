using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using DefaultTypes;
using DtoGenerator.DescriptionTypes;
using Newtonsoft.Json.Linq;
using TypeInterface;

namespace DtoGenerator
{
    public class DtoGenarator
    {

        internal List<IType> TypeList = new List<IType>();

        private string _folderPath;
        public DtoGenarator(string path, string folderPath)
        {
            _folderPath = folderPath;
            StartJob(path);
        }

        private void StartJob(string path)
        {
            var classList = DeserialiazeJson<List<ClassDescription>>(ReadJsonFromFile(path));
            LoadTypes();
            var generator = new ClassGenerator(classList, TypeList, "Test", _folderPath);
            generator.Generate();
        }


        private T DeserialiazeJson<T>(string json) where T : IEnumerable, new()
        {
            JToken jToken = JObject.Parse(json);
            var classList = jToken["classDescriptions"].ToObject<T>();
            return classList;
            ;
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


        private void LoadTypes()
        {
            TypeList.Add(new BooleanType());
            TypeList.Add(new ByteType());
            TypeList.Add(new DateType());
            TypeList.Add(new FloatType());
            TypeList.Add(new Integer32Type());
            TypeList.Add(new Integer64Type());
            TypeList.Add(new StringType());
            LoadAssemblies();
            //todo make assembly loader
        }


        private void LoadAssemblies()
        {
            IEnumerable<string> assembliesFiles;
            try
            {
                assembliesFiles = Directory.EnumerateFiles("plugins", "*.dll", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Cannot find plugin folder\nPlugin folder was created, put your plugins in it");
                Directory.CreateDirectory("plugins");
                return;
            }

            foreach (var assembly in assembliesFiles)
            {
                Assembly pluginAssembly;
                try
                {
                    pluginAssembly = Assembly.LoadFrom(assembly);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    continue;
                }
                Type[] typesInAssembly = pluginAssembly.GetExportedTypes();
                foreach (var type in typesInAssembly)
                {
                    if ((type.IsClass) && (typeof(IType).IsAssignableFrom(type)))
                    {
                        IType plugin = (IType) Activator.CreateInstance(type);
                        TypeList.Add(plugin);
                    }
                }
            }
        }
    }
}
