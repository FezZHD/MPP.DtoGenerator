using System;
using TypeInterface;

namespace DefaultTypes
{
    public class FloatType:IType
    {
        public string Name => "number";
        public string Format => "float";
        public Type Type => typeof(float);
    }
}