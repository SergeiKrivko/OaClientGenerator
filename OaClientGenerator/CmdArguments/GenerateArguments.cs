using CommandLine;

namespace OaClientGenerator.CmdArguments;

[Verb("generate", HelpText = "Генерация клиента")]
public class GenerateArguments
{
    [Option('i', "input", Required = true,
        HelpText = "Входной файл openapi.json или ссылка на него.")]
    public string? Input { get; set; }

    [Option('p', "project", Required = true,
        HelpText = "Имя проекта.")]
    public string? ProjectName { get; set; }

    [Option('n', "namespace", Required = false,
        HelpText = "Базовое пространство имен. По умолчанию используется '<Имя проекта>.Client'")]
    public string? Namespace { get; set; }

    [Option('o', "output", Required = false,
        HelpText = "Выходная папка. По умолчанию используется текущая")]
    public string? Output { get; set; }
}