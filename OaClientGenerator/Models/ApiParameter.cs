using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OaClientGenerator.Enums;

namespace OaClientGenerator.Models;

public class ApiParameter
{
    [JsonProperty("name")] public required string Name { get; init; }

    [JsonProperty("in")]
    [JsonConverter(typeof(StringEnumConverter))]
    public required ParameterPosition Position { get; init; }

    [JsonProperty("schema")] public required ApiSchema Schema { get; init; }

    [JsonProperty("required")] public bool Required { get; init; }
    [JsonProperty("description")] public required string Description { get; init; }
    [JsonProperty("title")] public required string Title { get; init; }
}