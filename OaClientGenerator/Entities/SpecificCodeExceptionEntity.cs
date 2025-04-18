using System.Net;
using OaClientGenerator.Abstraction;

namespace OaClientGenerator.Entities;

public class SpecificCodeExceptionEntity(HttpStatusCode httpStatusCode, string className, string baseException = "Exception") : ICodeEntity
{
    public string FileName => $"Exceptions/{className}.cs";

    public SpecificCodeExceptionEntity(HttpStatusCode httpStatusCode, string clientName) : this(
        httpStatusCode, $"{clientName}{httpStatusCode}Exception", $"{clientName}ErrorCodeException")
    {
    }

    public string WriteCode(string baseNamespace)
    {
        return $"namespace {baseNamespace}.Exceptions;\n" +
               $"\n" +
               $"public class {className} : {baseException}\n" +
               $"{{\n" +
               $"\n" +
               $"    public {className}() : base({(int)httpStatusCode})\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(Exception innerException) : base({(int)httpStatusCode}, innerException)\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"\n" +
               $"\n" +
               $"    public {className}(string message) : base({(int)httpStatusCode}, message)\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(string message, Exception innerException) : base({(int)httpStatusCode}, message, innerException)\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"}}";
    }
}