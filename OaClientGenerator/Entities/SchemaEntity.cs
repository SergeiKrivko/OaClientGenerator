using Humanizer;
using OaClientGenerator.Abstraction;
using OaClientGenerator.Models;
using OaClientGenerator.Services;

namespace OaClientGenerator.Entities;

public class SchemaEntity(string name, ApiSchema apiSchema) : ICodeEntity
{
    public string FileName => $"Models/{ClassName}.cs";
    private string ClassName => $"{name.Pascalize()}";

    private static string GenerateProperty(string name, ApiSchema property, bool isRequired)
    {
        var propertyType = property.GetTypeString();
        propertyType = isRequired ? $"required {propertyType}" : $"{propertyType.TrimEnd('?')}?";
        var res = $"[JsonProperty(\"{name}\")] public {propertyType} {name.Pascalize()} {{ get; init; }}";
        if (property.IsEnum())
            res = $"[JsonConverter(typeof({property.GetTypeString().TrimEnd('?')}))]" + res;
        return res;
    }

    public string WriteCode(string baseNamespace)
    {
        if (apiSchema.IsClass)
        {
            if (apiSchema.Properties == null)
                throw new Exception("No properties defined");
            return $"using Newtonsoft.Json;\n" +
                   $"\n" +
                   $"namespace {baseNamespace}.Models;\n" +
                   $"\n" +
                   $"public class {ClassName}\n" +
                   $"{{\n" +
                   $"    {string.Join("\n    ", apiSchema.Properties
                       .Select(item => GenerateProperty(item.Key, item.Value, apiSchema.Required.Contains(item.Key)))
                   )}\n" +
                   $"}}";
        }
        if (apiSchema.IsEnum)
        {
            if (apiSchema.Enum == null)
                throw new Exception("No enums defined");
            return $"using Newtonsoft.Json;\n" +
                   $"\n" +
                   $"namespace {baseNamespace}.Models;\n" +
                   $"\n" +
                   $"public enum {ClassName}\n" +
                   $"{{\n" +
                   $"    {string.Join(",\n    ", apiSchema.Enum.Select(e => $"[JsonProperty(\"{e}\")] {e.Pascalize()}"))}\n" +
                   $"}}";
        }

        throw new Exception();
    }
}