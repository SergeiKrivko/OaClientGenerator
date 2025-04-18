using System.Reflection;
using CommandLine;
using OaClientGenerator;
using OaClientGenerator.CmdArguments;

var types = LoadVerbs();

await Parser.Default.ParseArguments(args, types)
    .WithParsedAsync(Run);

return;

Type[] LoadVerbs() => Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();


async Task Run(object obj)
{
    switch (obj)
    {
        case GenerateArguments o:
            await ClientGenerator.GenerateClientAsync(o.Input!, o.Output ?? ".", o.ProjectName!, o.Namespace);
            break;
    }
}