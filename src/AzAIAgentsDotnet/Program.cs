using AzAIAgentsDotnet.DependencyInjecion;
using AzAIAgentsDotnet.Models;
using AzAIAgentsDotnet.Services;
using AzAIAgentsDotnet.Utils;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace AzAIAgentsDotnet;

internal class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.ConfiureServices();
        var provider = services.BuildServiceProvider();

        DisplayWelcomeMessage();

        var agentFactory = provider.GetRequiredService<IAgentExecutionServiceFactory>();

        while (true)
        {
            DisplayAvailablePrompts();

            var selectedPrompt = GetSelectedPrompt();
            if (selectedPrompt == null) break;

            var agentService = agentFactory.GetAgentService(selectedPrompt);
            await AnsiConsole.Status()
                .StartAsync("Running prompt...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    agentService.ProgressUpdated += (sender, message) =>
                    {
                        ctx.Status(message);
                    };

                    AnsiConsole.WriteLine();
                    AnsiConsole.WriteLine($"Prompt: {selectedPrompt.Prompt}");
                    await agentService.ExecuteAsync(selectedPrompt);
                });

        }
    }

    private static void DisplayWelcomeMessage()
    {
        AnsiConsole.Write(
            new FigletText("Azure AI Agents Dotnet")
                .LeftJustified()
                .Color(Color.Blue));
    }

    private static void DisplayAvailablePrompts()
    {
        AnsiConsole.WriteLine("Available Prompts:");
        var table = new Table();
        table.AddColumn("Number");
        table.AddColumn("Category");
        table.AddColumn("Prompt");

        var counter = 0;
        var keys = PromptConfigData.PromptConfigurations.Keys;
        foreach (var key in keys)
        {
            var item = PromptConfigData.PromptConfigurations[key];
            table.AddRow($"{++counter}", key, $"{item.Prompt}");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static PromptConfig? GetSelectedPrompt()
    {
        var keys = PromptConfigData.PromptConfigurations.Keys;

        var selection = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter prompt number to run [green](or 0 to exit)[/]:")
                .ValidationErrorMessage("[red]Please enter a valid sample number[/]")
                .Validate(num =>
                {
                    if (num < 0) return ValidationResult.Error("[red]Number must be positive[/]");
                    if (num > keys.Count) return ValidationResult.Error($"[red]Must be between 0 and {keys.Count}[/]");
                    return ValidationResult.Success();
                }));

        if (selection == 0) return null;

        var selectedKey = keys.ElementAt(selection - 1);
        return PromptConfigData.PromptConfigurations[selectedKey];
    }
}