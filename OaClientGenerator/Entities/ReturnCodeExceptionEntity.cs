using OaClientGenerator.Abstraction;

namespace OaClientGenerator.Entities;

public class ReturnCodeExceptionEntity(string className, string baseException = "Exception") : ICodeEntity
{
    public string FileName => $"Exceptions/{className}.cs";

    public string WriteCode(string baseNamespace)
    {
        return $"namespace {baseNamespace}.Exceptions;\n" +
               $"\n" +
               $"public class {className} : {baseException}\n" +
               $"{{\n" +
               $"\n" +
               $"    public int Code {{ get; }}\n" +
               $"\n" +
               $"    public {className}(int code) : base($\"Api returns error code {{code}}\")\n" +
               $"    {{\n" +
               $"        Code = code;\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(int code, Exception innerException) : base($\"Api returns error code {{code}}\", innerException)\n" +
               $"    {{\n" +
               $"        Code = code;\n" +
               $"    }}\n" +
               $"\n" +
               $"\n" +
               $"    public {className}(int code, string message) : base(message)\n" +
               $"    {{\n" +
               $"        Code = code;\n" +
               $"    }}\n" +
               $"\n" +
               $"    public {className}(int code, string message, Exception innerException) : base(message, innerException)\n" +
               $"    {{\n" +
               $"        Code = code;\n" +
               $"    }}\n" +
               $"}}";
    }
}