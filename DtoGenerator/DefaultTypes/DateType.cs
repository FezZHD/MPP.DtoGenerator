using System;
using TypeInterface;

namespace DefaultTypes
{
    public class DateType:IType
    {
        public string Name => "string";
        public string Format => "date";
        public Type Type => typeof(DateTime);
    }
}