using Newtonsoft.Json;

namespace OaClientGenerator.Enums;

public enum ParameterPosition
{
    [JsonProperty("path")] Path,
    [JsonProperty("query")] Query,
}