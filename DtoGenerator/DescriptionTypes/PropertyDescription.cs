using System;
using Newtonsoft.Json;
using TypeInterface;

namespace DtoGenerator.DescriptionTypes
{
    internal class PropertyDescription: IType
    {
        [JsonProperty("name")]
        public string Name { get; }
        [JsonProperty("format")]
        public string Format { get; }
        [JsonProperty("type")]
        public Type Type { get; }

        internal PropertyDescription(string name, string format, Type type)
        {
            Name = name;
            Format = format;
            Type = type;
        }
    }
}