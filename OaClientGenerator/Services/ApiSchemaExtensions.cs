using OaClientGenerator.Models;

namespace OaClientGenerator.Services;

public static class ApiSchemaExtensions
{
    public static string GetTypeString(this ApiSchema apiSchema)
    {
        if (apiSchema.Union != null)
            return apiSchema.Union.First(e => e.Type != "null").GetTypeString() + "?";
        if (apiSchema.Ref != null || apiSchema.IsClass || apiSchema.IsEnum)
        {
            return SchemasService.Instance.GetSchema(apiSchema);
        }

        if (apiSchema.IsArray)
        {
            return $"{apiSchema.ArrayType?.GetTypeString()}[]";
        }

        return apiSchema.Format switch
        {
            "date" => nameof(DateOnly),
            "date-time" => nameof(DateTime),
            "time" => nameof(TimeOnly),
            _ => apiSchema.Type switch
            {
                "string" => "string",
                "integer" => "int",
                "int" => "int",
                "boolean" => "bool",
                "bool" => "bool",
                _ => "object"
            }
        };
    }

    public static bool IsEnum(this ApiSchema apiSchema)
    {
        if (apiSchema.Ref != null)
            return SchemasService.Instance.RefIsEnum(apiSchema.Ref);
        return apiSchema.IsEnum;
    }

    public static bool IsClass(this ApiSchema apiSchema)
    {
        if (apiSchema.Ref != null)
            return SchemasService.Instance.RefIsClass(apiSchema.Ref);
        return apiSchema.IsClass;
    }

    public static bool IsArray(this ApiSchema apiSchema)
    {
        if (apiSchema.Ref != null)
            return SchemasService.Instance.RefIsArray(apiSchema.Ref);
        return apiSchema.IsArray;
    }

    public static bool IsBuiltIn(this ApiSchema apiSchema)
    {
        if (apiSchema.Union != null)
            return apiSchema.Union.All(e => e.IsBuiltIn());
        if (apiSchema.IsClass())
            return false;
        if (apiSchema.IsEnum())
            return false;
        if (apiSchema.IsArray() && apiSchema.ArrayType != null)
            return apiSchema.ArrayType.IsBuiltIn();
        return true;
    }
}