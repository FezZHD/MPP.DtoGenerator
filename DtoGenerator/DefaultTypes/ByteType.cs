using System;
using TypeInterface;

namespace DefaultTypes
{
    public class ByteType:IType
    {
        public string Name => "string";
        public string Format => "byte";
        public Type Type => typeof(byte);
    }
}