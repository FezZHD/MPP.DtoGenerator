using System;
using TypeInterface;

namespace DefaultTypes
{
    public class BooleanType:IType
    {
        public string Name => "boolean";
        public string Format => String.Empty;
        public Type Type => typeof(bool);
    }
}