using System;
using System.Collections.Generic;

namespace DtoGenerator
{
    internal sealed class Logger
    {
        private static Lazy<Logger> lazy = new Lazy<Logger>(()=> new Logger());

        internal static Logger Instance { get { return lazy.Value; } }

        private readonly List<Exception> exceptionList = new List<Exception>();

        private Logger()
        {
            
        }


        internal void GetException(Exception currentException)
        {
            exceptionList.Add(currentException);
        }


        internal void PrintExceptions()
        {
            foreach (var exception in exceptionList)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}