namespace OaClientGenerator.Abstraction;

public interface IMethodEntity
{
    public string WriteCode(int baseIndent);
    public IEnumerable<string> RequiredUsings => [];
}