using Humanizer;
using OaClientGenerator.Abstraction;
using OaClientGenerator.Enums;
using OaClientGenerator.Models;
using OaClientGenerator.Services;

namespace OaClientGenerator.Entities;

public class ApiPathMethod : IMethodEntity
{
    private readonly string _endpoint;
    private readonly string _method;
    public ApiPath ApiPath { get; }

    public HashSet<string> RequiredUsings { get; } = [];

    public ApiPathMethod(string endpoint, string method, ApiPath apiPath)
    {
        _endpoint = endpoint;
        _method = method;
        ApiPath = apiPath;
    }

    private string GenerateName()
    {
        List<string> result = [_method];
        var pathItems = _endpoint.Split('/').Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

        for (var i = 0; i < pathItems.Length; i++)
        {
            if (pathItems[i].StartsWith('{'))
            {
                if (i > 0 && IsPlural(pathItems[i - 1]))
                    result[^1] = result[^1].Singularize(false);
                if (i == pathItems.Length - 1)
                {
                    result.Add("by");
                    result.Add(PathParamName(pathItems[i], i > 0 ? pathItems[i - 1] : null));
                }
            }
            else
            {
                result.Add(pathItems[i]);
            }
        }

        return string.Join(' ', result).Pascalize();
    }

    private static bool IsPlural(string word)
    {
        return string.Equals(word, word.Pluralize(), StringComparison.OrdinalIgnoreCase);
    }

    private static string PathParamName(string paramName, string? previousParamName)
    {
        paramName = paramName.TrimStart('{').TrimEnd('}');
        if (previousParamName != null && paramName.ToLower().StartsWith(previousParamName.Singularize(false).ToLower()))
        {
            paramName = paramName.Substring(previousParamName.Length);
        }

        return paramName.Trim('_').Trim();
    }

    private static string Indent(int indent) => string.Join(string.Empty, Enumerable.Repeat("    ", indent));

    private string GenerateReturnType()
    {
        RequiredUsings.Add("System.Threading.Tasks");
        var response = ApiPath.Responses["200"];
        if (response.Content.Json == null)
            return "Task";
        var schemaName = SchemasService.Instance.GetSchema(response.Content.Json.Schema, GenerateName() + "Response");
        return $"Task<{schemaName}>";
    }

    private string GenerateRequestCall()
    {
        var call = $"{_method.Pascalize()}Async";
        var response = ApiPath.Responses["200"];
        if (response.Content.Json != null)
            call = $"return await {call}<{response.Content.Json.Schema.GetTypeString()}>";
        else
            call = $"await {call}";
        return call;
    }

    private record MethodParameter(string Name, string ApiName, string Type, string? Converter, string? DefaultValue)
    {
        public string InMethod => $"{Type} {Name}" + (DefaultValue == null ? "" : $" = {DefaultValue}");
    }

    private IEnumerable<MethodParameter> ProcessPathParams()
    {
        return ApiPath.Parameters
            .Where(p => p.Position == ParameterPosition.Path)
            .Select(ProcessParam);
    }

    private IEnumerable<MethodParameter> ProcessQueryParams()
    {
        return ApiPath.Parameters
            .Where(p => p.Position == ParameterPosition.Query)
            .Select(ProcessParam);
    }

    private MethodParameter ProcessParam(ApiParameter parameter)
    {
        var type = parameter.Schema.GetTypeString();
        if (!parameter.Schema.IsBuiltIn())
            type = $"Models.{type}";

        RequiredUsings.Add("System");
        return new MethodParameter(parameter.Name.Camelize(), parameter.Name, type,
            type switch
            {
                "DateTime" => $"{parameter.Name.Camelize()}.ToString(\"o\")",
                "DateTime?" => $"{parameter.Name.Camelize()}.Value.ToString(\"o\")",
                _ => $"Convert.ToString({parameter.Name.Camelize()})"
            },
            type.EndsWith('?') ? "null" : parameter.Schema.Default?.ToString());
    }

    public string WriteCode(int baseIndent)
    {
        var pathParams = ProcessPathParams().ToArray();
        var queryParams = ProcessQueryParams().ToArray();
        var allParams = pathParams.Concat(queryParams);
        var url = _endpoint;
        foreach (var pathParam in pathParams)
        {
            url = url.Replace("{" + pathParam.ApiName + "}", "{" + pathParam.Name + "}");
        }

        return $"{Indent(baseIndent)}/// <summary>\n" +
               $"{Indent(baseIndent)}/// {ApiPath.Summary} \n" +
               $"{Indent(baseIndent)}/// </summary>\n" +
               $"{string.Join('\n', ApiPath.Parameters
                   .Select(p => $"{Indent(baseIndent)}/// <param name=\"{p.Name.Camelize()}\">{p.Description}</param>")
               )}\n" +
               $"{Indent(baseIndent)}/// <param name=\"cancellationToken\">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>\n" +
               $"{Indent(baseIndent)}public async {GenerateReturnType()} {GenerateName()}" +
               $"({string.Join(", ", allParams
                   .Select(p => p.InMethod)
               )}, CancellationToken? cancellationToken = null)\n" +
               $"{Indent(baseIndent)}{{\n" +
               $"{Indent(baseIndent)}    Dictionary<string, string?> query = [];\n" +
               $"{Indent(baseIndent)}    {string.Join("\n" + Indent(baseIndent + 1), queryParams
                   .Select(GenerateParamCheck)
               )}\n" +
               $"{Indent(baseIndent)}    {GenerateRequestCall()}($\"{url}\", query, cancellationToken ?? CancellationToken.None);\n" +
               $"{Indent(baseIndent)}}}\n";
    }

    private static string GenerateParamCheck(MethodParameter parameter)
    {
        var res = $"query[\"{parameter.ApiName}\"] = {parameter.Converter ?? parameter.Name};";
        if (parameter.Type.EndsWith('?'))
            res = $"if ({parameter.Name} != null) " + res;
        return res;
    }
}