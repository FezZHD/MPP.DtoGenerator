using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtoGenerator;

namespace DtoGeneratorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (File.Exists(args[0]))
                {
                    if (!Directory.Exists(args[1]))
                    {
                        Directory.CreateDirectory(args[1]);
                    }
                    Console.WriteLine("K");
                    var generator = new DtoGenarator(args[0], args[1]);
                }
                else
                {
                    Console.WriteLine($"file {args[0]} doesn't exist");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Missing file path or output folder path");
            }
            finally
            {
                Console.ReadKey();
            }           
        }
    }
}
