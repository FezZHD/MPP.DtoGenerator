using System;
using TypeInterface;

namespace TypePlugin
{
    public class DecimalType :IType
    {
        public string Name => "number";
        public string Format => "decimal";
        public Type Type => typeof(decimal);
    }
}
