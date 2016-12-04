using System;

namespace DtoGenerator
{
    public interface ILogger
    {
        void GetException(Exception currentException);
        void PrintExceptions();
    }
}