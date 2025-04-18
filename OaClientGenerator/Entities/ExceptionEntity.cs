using OaClientGenerator.Abstraction;

namespace OaClientGenerator.Entities;

public class ExceptionEntity(string className, string baseException = "Exception") : ICodeEntity
{
    public string FileName => $"Exceptions/{className}.cs";

    public string WriteCode(string baseNamespace)
    {
        return $"namespace {baseNamespace}.Exceptions;\n" +
               $"\n" +
               $"public class {className} : {baseException}\n" +
               $"{{\n" +
               $"    public {className}()\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(string message) : base(message)\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(string message, Exception innerException) : base(message, innerException)\n" +
               $"    {{\n" +
               $"    }}\n" +
               $"}}";
    }
}