using System;
using TypeInterface;

namespace DefaultTypes
{
    public class Integer32Type: IType
    {
        public string Name => "integer";
        public string Format => "int32";
        public Type Type => typeof(Int32);
    }
}
