using Newtonsoft.Json;

namespace OaClientGenerator.Models;

public class ApiPath
{
    [JsonProperty("tags")] public string[] Tags { get; init; } = [];
    [JsonProperty("summary")] public string Summary { get; init; } = string.Empty;
    [JsonProperty("operationId")] public string OperationId { get; init; } = string.Empty;
    [JsonProperty("parameters")] public ApiParameter[] Parameters { get; init; } = [];
    [JsonProperty("responses")] public Dictionary<string, ApiResponse> Responses { get; init; } = [];

    [JsonIgnore] string? Tag => Tags.FirstOrDefault();
}