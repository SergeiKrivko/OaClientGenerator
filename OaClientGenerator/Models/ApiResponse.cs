using Newtonsoft.Json;

namespace OaClientGenerator.Models;

public class ApiResponse
{
    [JsonProperty("description")] public string? Description { get; init; }
    [JsonProperty("content")] public required ApiResponseContent Content { get; init; }

    public class ApiResponseContent
    {
        [JsonProperty("application/json")] public JsonResponse? Json { get; init; }
        [JsonProperty("id")] public required string Id { get; init; }
    }

    public class JsonResponse
    {
        [JsonProperty("schema")] public required ApiSchema Schema { get; init; }
        [JsonProperty("id")] public required int Id { get; init; }
    }
}