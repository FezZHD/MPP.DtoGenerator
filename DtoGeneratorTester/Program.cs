using System;
using System.IO;
using System.Configuration;
using DtoGenerator;

namespace DtoGeneratorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var namespaceName = ConfigurationManager.AppSettings["Namespace"];
            var maximumTaskCount = ConfigurationManager.AppSettings["MaximumPoolTasks"];
            try
            {
                int maximumTaskCountInt = Int32.Parse(maximumTaskCount);
                if (!String.IsNullOrWhiteSpace(namespaceName))
                {
                    if (File.Exists(args[0]))
                    {
                        if (!Directory.Exists(args[1]))
                        {
                            Directory.CreateDirectory(args[1]);
                        }
                        Console.WriteLine("K");
                        var generator = new DtoGenarator(args[0], args[1], maximumTaskCountInt, namespaceName);
                        generator.StartJob();
                    }
                    else
                    {
                        Console.WriteLine($"file {args[0]} doesn't exist");
                    }
                }
                else
                {
                    Console.WriteLine("Miss Namespace name");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Missing file path or output folder path");
            }
            catch (FormatException)
            {
                Console.WriteLine("Error number for thread count in config, please, remove all spaces");
            }
            finally
            {
                Console.ReadKey();
            }           
        }
    }
}
