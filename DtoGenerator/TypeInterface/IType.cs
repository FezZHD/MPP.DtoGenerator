using System;

namespace TypeInterface
{
    public interface IType
    {
        string Name { get; }
        string Format { get; }
        Type Type { get; }
    }
}
