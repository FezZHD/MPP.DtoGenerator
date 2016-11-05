using System;
using TypeInterface;

namespace DefaultTypes
{
    class DoubleType : IType
    {
        public string Name => "number";
        public string Format => "double";
        public Type Type => typeof(double);
    }
}
