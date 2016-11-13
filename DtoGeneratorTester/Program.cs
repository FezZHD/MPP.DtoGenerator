﻿using System;
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
                    Console.WriteLine("K");
                    var generator = new DtoGenarator(args[0]);
                }
                else
                {
                    Console.WriteLine($"file {args[0]} doesn't exist");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Missing file path");
            }
            finally
            {
                Console.ReadKey();
            }           
        }
    }
}
