using System;
using TypeInterface;

namespace TypePlugin
{
    public class UInt32Type:IType
    {
        public string Name => "integer";
        public string Format => "uint32";
        public Type Type => typeof(UInt32);
    }
}