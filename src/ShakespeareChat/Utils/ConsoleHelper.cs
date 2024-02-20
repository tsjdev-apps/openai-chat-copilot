using Spectre.Console;

namespace ShakespeareChat.Utils;

internal static class ConsoleHelper
{
    public static void CreateHeader()
    {
        AnsiConsole.Clear();

        Grid grid = new();
        grid.AddColumn();
        grid.AddRow(new FigletText("ShakespeareChat").Centered().Color(Color.Red));
        grid.AddRow(Align.Center(new Panel("[red]Sample by Thomas Sebastian Jensen ([link]https://www.tsjdev-apps.de[/])[/]")));

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();
    }

    public static string GetHoster()
        => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select the [yellow]hoster[/]?")
            .AddChoices([Statics.OpenAIKey, Statics.AzureOpenAIKey]));

    public static string GetAzureOpenAIEndpoint()
        => AnsiConsole.Prompt(
            new TextPrompt<string>("Please insert your [yellow]Azure OpenAI endpoint[/]:")
            .PromptStyle("white")
            .ValidationErrorMessage("[red]Invalid prompt[/]")
            .Validate(prompt =>
            {
                if (prompt.Length < 3)
                {
                    return ValidationResult.Error("[red]Endpoint to short[/]");
                }

                if (prompt.Length > 250)
                {
                    return ValidationResult.Error("[red]Endpoint to long[/]");
                }

                if (Uri.TryCreate(prompt, UriKind.Absolute, out var uri)
                    && uri.Scheme == Uri.UriSchemeHttps)
                {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error("[red]No valid URL[/]");
            }));

    public static string GetApiKey(string type)
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"Please insert your [yellow]{type}[/] API key:")
            .PromptStyle("white")
            .ValidationErrorMessage("[red]Invalid prompt[/]")
            .Validate(prompt =>
            {
                if (prompt.Length < 3)
                {
                    return ValidationResult.Error("[red]API key too short[/]");
                }

                if (prompt.Length > 200)
                {
                    return ValidationResult.Error("[red]API key too long[/]");
                }

                return ValidationResult.Success();
            }));

    public static string GetDeploymentName()
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"Please insert the [yellow]deployment name[/] of the model:")
            .PromptStyle("white")
            .ValidationErrorMessage("[red]Invalid prompt[/]")
            .Validate(prompt =>
            {
                if (prompt.Length < 3)
                {
                    return ValidationResult.Error("[red]Deployment name too short[/]");
                }

                if (prompt.Length > 200)
                {
                    return ValidationResult.Error("[red]Deployment name too long[/]");
                }

                return ValidationResult.Success();
            }));
}
