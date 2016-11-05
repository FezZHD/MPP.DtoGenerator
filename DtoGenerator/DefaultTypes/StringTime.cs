using System;
using TypeInterface;

namespace DefaultTypes
{
    public class StringTime:IType
    {
        public string Name => "string";
        public string Format => "string";
        public Type Type => typeof(string);
    }
}