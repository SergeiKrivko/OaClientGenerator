using System.Net;
using Newtonsoft.Json;
using OaClientGenerator.Abstraction;
using OaClientGenerator.Entities;
using OaClientGenerator.Models;
using OaClientGenerator.Services;

namespace OaClientGenerator;

public class ClientGenerator
{
    private readonly OpenApi _openApi;
    private readonly string _clientName;
    private readonly string _baseNamespace;
    private readonly string _outputDirectory;

    public static HttpStatusCode[] StatusCodes { get; } =
    [
        HttpStatusCode.NotFound,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadRequest,
        HttpStatusCode.Unauthorized,
        HttpStatusCode.BadGateway,
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.NotAcceptable,
        HttpStatusCode.NotImplemented,
        HttpStatusCode.MethodNotAllowed,
        HttpStatusCode.Conflict,
    ];

    private ClientGenerator(OpenApi openApi, string outputPath, string clientName, string? baseNamespace = null)
    {
        _openApi = openApi;
        _clientName = clientName;
        _outputDirectory = outputPath;
        _baseNamespace = baseNamespace ?? $"{_clientName}.Client";
    }

    public static async Task GenerateClientAsync(string inputPath, string outputPath, string clientName, string? baseNamespace)
    {
        var openApiJson = await File.ReadAllTextAsync(inputPath);
        var openApi = JsonConvert.DeserializeObject<OpenApi>(openApiJson.Replace("\"$ref\"", "\"ref\""));
        if (openApi == null)
            throw new FileNotFoundException("Could not deserialize open API file.");
        await new ClientGenerator(openApi, outputPath, clientName, baseNamespace).GenerateClientAsync();
    }

    private async Task GenerateClientAsync()
    {
        if (_openApi.Components != null)
            SchemasService.Instance.DefineSchemas(_openApi.Components);

        await GenerateEntityAsync(new ClientCodeEntity(_openApi.Paths, _clientName));

        foreach (var codeEntity in SchemasService.Instance.Generate())
        {
            await GenerateEntityAsync(codeEntity);
        }

        foreach (var codeEntity in GenerateExceptions())
        {
            await GenerateEntityAsync(codeEntity);
        }
    }

    private async Task GenerateEntityAsync(ICodeEntity codeEntity)
    {
        Directory.CreateDirectory(Path.Join(_outputDirectory, Path.GetDirectoryName(codeEntity.FileName)));
        await File.WriteAllTextAsync(Path.Join(_outputDirectory, codeEntity.FileName),
            codeEntity.WriteCode(_baseNamespace));
    }

    private IEnumerable<ICodeEntity> GenerateExceptions()
    {
        yield return new ExceptionEntity($"{_clientName}Exception");
        yield return new ExceptionEntity($"{_clientName}JsonException", $"{_clientName}Exception");
        yield return new ExceptionEntity($"{_clientName}ConnectionException", $"{_clientName}Exception");
        yield return new ReturnCodeExceptionEntity($"{_clientName}ErrorCodeException", $"{_clientName}Exception");
        foreach (var statusCode in StatusCodes)
        {
            yield return new SpecificCodeExceptionEntity(statusCode, _clientName);
        }
    }
}