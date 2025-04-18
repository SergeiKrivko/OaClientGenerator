using OaClientGenerator.Abstraction;
using OaClientGenerator.Models;

namespace OaClientGenerator.Entities;

public class ClientCodeEntity(Dictionary<string, Dictionary<string, ApiPath>> paths, string clientName) : ICodeEntity
{
    public string FileName => $"{clientName}Client.cs";

    private readonly HashSet<string> _usings = [];

    private IEnumerable<ApiPathMethod> GetAllPaths()
    {
        foreach (var (endpoint, item) in paths)
        {
            foreach (var (method, apiPath) in item)
            {
                yield return new ApiPathMethod(endpoint, method, apiPath);
            }
        }
    }

    public string WriteCode(string baseNamespace)
    {
        var endpointGroups = GetAllPaths()
            .GroupBy(e => e.ApiPath.Tags.First())
            .ToArray();
        var methods = endpointGroups.Select(g => new
            { g.Key, MethodsCode = string.Join("\n", g.Select(e => e.WriteCode(1))) });
        foreach (var endpointGroup in endpointGroups)
        {
            foreach (var apiPathMethod in endpointGroup)
            {
                foreach (var u in apiPathMethod.RequiredUsings)
                {
                    _usings.Add(u);
                }
            }
        }

        return $"using System.Net;\n" +
               $"using System.Net.Http.Json;\n" +
               $"using System.Text.Json;\n" +
               $"using System.Text.Json.Serialization;\n" +
               $"{string.Join("\n", _usings.Select(u => $"using {u};"))}\n" +
               $"\n" +
               $"using {baseNamespace}.Models;\n" +
               $"using {baseNamespace}.Exceptions;\n" +
               $"\n" +
               $"namespace {baseNamespace};\n" +
               $"\n" +
               $"public class {clientName}Client\n" +
               $"{{\n" +
               $"    private readonly HttpClient _httpClient;\n" +
               $"    private readonly JsonSerializerOptions _jsonOptions;\n" +
               $"\n" +
               $"    public {clientName}Client(string baseUrl)\n" +
               $"    {{\n" +
               $"        _httpClient = new HttpClient() {{ BaseAddress = new Uri(baseUrl.TrimEnd('/')) }};\n" +
               $"        _jsonOptions = new JsonSerializerOptions\n" +
               $"        {{\n" +
               $"            PropertyNameCaseInsensitive = true,\n" +
               $"            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull\n" +
               $"        }};\n" +
               $"    }}\n" +
               $"\n" +
               $"{string.Join("\n", methods
                   .Select(g => $"    #region {g.Key}\n" +
                                $"\n" +
                                $"{string.Join("\n", g.MethodsCode)}\n" +
                                $"    #endregion\n")
               )}\n" +
               $"\n" +
               $"    #region Private helpers\n" +
               $"\n" +
               $"    private static string GenerateUrl(string url, Dictionary<string, string?> queryParams)\n" +
               $"    {{\n" +
               $"        return url + \"?\" + string.Join(\"&\", queryParams.Select(p => $\"{{p.Key}}={{Uri.EscapeDataString(p.Value ?? \"\")}}\"));\n" +
               $"    }}\n" +
               $"\n" +
               $"    private async Task<T> GetAsync<T>(string url, Dictionary<string, string?> query, CancellationToken cancellationToken)\n" +
               $"    {{\n" +
               $"        HttpResponseMessage response;\n" +
               $"        try\n" +
               $"        {{\n" +
               $"            response = await _httpClient.GetAsync(GenerateUrl(url, query), cancellationToken);\n" +
               $"        }}\n" +
               $"        catch (HttpRequestException e)\n" +
               $"        {{\n" +
               $"            throw new {clientName}ConnectionException(e.Message, e);\n" +
               $"        }}\n" +
               $"\n" +
               $"        if (!response.IsSuccessStatusCode)\n" +
               $"        {{\n" +
               $"            throw response.StatusCode switch\n" +
               $"            {{\n" +
               $"                {string.Join(",\n                ", ClientGenerator.StatusCodes
                   .Select(code => $"HttpStatusCode.{code} => new {clientName}{code}Exception()"))},\n" +
               $"                _ => new {clientName}ErrorCodeException((int)response.StatusCode),\n" +
               $"            }};\n" +
               $"        }}\n" +
               $"\n" +
               $"        var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);\n" +
               $"        if (result == null)\n" +
               $"        {{\n" +
               $"            throw new {clientName}JsonException($\"Failed to deserialize {{typeof(T).Name}}\");\n" +
               $"        }}\n" +
               $"        return result;\n" +
               $"    }}\n" +
               $"\n" +
               $"    #endregion\n" +
               $"}}";
    }
}