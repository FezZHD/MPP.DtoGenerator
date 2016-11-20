using Newtonsoft.Json;

namespace DtoGenerator.DescriptionTypes
{
    internal class PropertyDescription
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("format")]
        internal string Format { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }
    }
}