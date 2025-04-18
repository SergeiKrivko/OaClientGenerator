using Newtonsoft.Json;

namespace OaClientGenerator.Models;

public class OpenApi
{
    [JsonProperty("openapi")] public required Version OpenApiVersion { get; init; }
    [JsonProperty("paths")] public required Dictionary<string, Dictionary<string, ApiPath>> Paths { get; init; }
    [JsonProperty("components")] public ApiComponents? Components { get; init; }
}