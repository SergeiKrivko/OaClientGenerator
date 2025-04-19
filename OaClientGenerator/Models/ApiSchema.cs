using Newtonsoft.Json;

namespace OaClientGenerator.Models;

public class ApiSchema
{
    // [JsonProperty("title")] public string? Title { get; init; }
    // [JsonProperty("description")] public string? Description { get; init; }

    [JsonProperty("type")] public string? Type { get; init; }
    [JsonProperty("format")] public string? Format { get; init; }
    [JsonProperty("anyOf")] public ApiSchema[]? Union { get; init; }

    [JsonProperty("ref")] public string? Ref { get; init; }

    [JsonProperty("enum")] public string[]? Enum { get; init; }

    [JsonProperty("properties")] public Dictionary<string, ApiSchema>? Properties { get; init; }

    [JsonProperty("items")] public ApiSchema? ArrayType { get; init; }

    [JsonIgnore] public bool IsClass => Type == "object" && Properties != null;
    [JsonIgnore] public bool IsEnum => Enum != null;
    [JsonIgnore] public bool IsArray => Type == "array" && ArrayType != null;

    [JsonProperty("default")] public string? Default { get; init; }

    [JsonProperty("required")] public string[] Required { get; init; } = [];
}