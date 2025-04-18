using Newtonsoft.Json;

namespace OaClientGenerator.Models;

public class ApiComponents
{
    [JsonProperty("schemas")] public Dictionary<string, ApiSchema> Schemas { get; init; } = [];
}