using System;
using System.Collections.Generic;

namespace DtoGenerator
{
    internal sealed class Logger:ILogger
    {
        private static Lazy<Logger> lazy = new Lazy<Logger>(()=> new Logger());

        internal static Logger Instance { get { return lazy.Value; } }

        private readonly List<Exception> exceptionList = new List<Exception>();

        private readonly object lockObject = new object();

        private Logger()
        {
            
        }


        public void GetException(Exception currentException)
        {
            lock (lockObject)
            {
                exceptionList.Add(currentException);
            }  
        }


        public void PrintExceptions()
        {
            foreach (var exception in exceptionList)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}