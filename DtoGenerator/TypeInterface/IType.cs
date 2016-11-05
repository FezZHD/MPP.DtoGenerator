using System;

namespace TypeInterface
{
    public interface IType
    {
        string Name { get; set; }
        string Format { get; set; }
        Type Type { get; set; }
    }
}
