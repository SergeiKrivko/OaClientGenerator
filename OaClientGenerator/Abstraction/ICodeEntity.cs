namespace OaClientGenerator.Abstraction;

public interface ICodeEntity
{
    public string FileName { get; }
    public string WriteCode(string baseNamespace);
}