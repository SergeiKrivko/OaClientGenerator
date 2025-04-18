using OaClientGenerator.Abstraction;
using OaClientGenerator.Entities;
using OaClientGenerator.Models;

namespace OaClientGenerator.Services;

public class SchemasService
{
    private static SchemasService? _instance;
    public static SchemasService Instance => _instance ??= new SchemasService();

    private readonly Dictionary<string, ApiSchema> _schemas = [];

    public string GetSchema(ApiSchema apiSchema, string? generatedSchemaName = null)
    {
        if (apiSchema.Ref != null)
        {
            if (!apiSchema.Ref.StartsWith("#/components/schemas/"))
                throw new Exception("Schema ref must start with '#/components/schemas/'");
            return apiSchema.Ref.Replace("#/components/schemas/", "");
        }

        generatedSchemaName ??= "Anonymous";
        if (_schemas.ContainsKey(generatedSchemaName))
        {
            var i = 1;
            while (_schemas.ContainsKey(generatedSchemaName + i))
            {
                i++;
            }

            generatedSchemaName += i;
        }

        _schemas[generatedSchemaName] = apiSchema;
        return generatedSchemaName;
    }

    public string GetSchema(string reference)
    {
        if (!reference.StartsWith("#/components/schemas/"))
            throw new Exception("Schema ref must start with '#/components/schemas/'");
        return reference.Replace("#/components/schemas/", "");
    }

    public bool RefIsClass(string reference)
    {
        if (!reference.StartsWith("#/components/schemas/"))
            throw new Exception("Schema ref must start with '#/components/schemas/'");
        reference = reference.Replace("#/components/schemas/", "");
        return _schemas[reference].IsClass;
    }

    public bool RefIsArray(string reference)
    {
        if (!reference.StartsWith("#/components/schemas/"))
            throw new Exception("Schema ref must start with '#/components/schemas/'");
        reference = reference.Replace("#/components/schemas/", "");
        return _schemas[reference].IsArray;
    }

    public bool RefIsEnum(string reference)
    {
        if (!reference.StartsWith("#/components/schemas/"))
            throw new Exception("Schema ref must start with '#/components/schemas/'");
        reference = reference.Replace("#/components/schemas/", "");
        return _schemas[reference].IsEnum;
    }

    public void DefineSchema(string name, ApiSchema apiSchema)
    {
        _schemas.Add(name, apiSchema);
    }

    public void DefineSchemas(ApiComponents apiComponents)
    {
        foreach (var (name, apiSchema) in apiComponents.Schemas)
        {
            DefineSchema(name, apiSchema);
        }
    }

    public IEnumerable<ICodeEntity> Generate()
    {
        foreach (var (name, apiSchema) in _schemas)
        {
            yield return new SchemaEntity(name, apiSchema);
        }
    }
}