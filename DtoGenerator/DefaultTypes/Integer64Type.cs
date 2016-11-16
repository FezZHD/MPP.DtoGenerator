using System;
using TypeInterface;

namespace DefaultTypes
{
    public class Integer64Type :IType
    {
        public string Name => "integer";
        public string Format => "int64";
        public Type Type => typeof(Int64);
    }
}
