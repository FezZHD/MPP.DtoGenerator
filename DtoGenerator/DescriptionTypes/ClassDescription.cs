using System.Collections.Generic;
using Newtonsoft.Json;

namespace DtoGenerator.DescriptionTypes
{
    internal class ClassDescription
    {
        [JsonProperty("className")]
        internal string ClassName { get; set; }

        [JsonProperty("properties")]
        internal List<PropertyDescription> Properties { get; set; }
    }
}