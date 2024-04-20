using Azure;
using Azure.AI.OpenAI;
using ShakespeareChat.Utils;
using Spectre.Console;
using System.Text;

// Create header
ConsoleHelper.CreateHeader();

// Get Host
string host = ConsoleHelper.SelectFromOptions([Statics.OpenAIKey, Statics.AzureOpenAIKey]);

// OpenAI Client
OpenAIClient? client = null;
string deploymentName = "gpt-3.5-turbo";

switch (host)
{
    case Statics.OpenAIKey:
        // Get OpenAI Key
        string openAIKey = 
            ConsoleHelper.GetString($"Please insert your [yellow]{Statics.OpenAIKey}[/] API key:");

        // Create OpenAI client
        client = new(openAIKey);

        break;

    case Statics.AzureOpenAIKey:
        // Get Endpoint
        string endpoint = 
            ConsoleHelper.GetUrl("Please insert your [yellow]Azure OpenAI endpoint[/]:");

        // Get Azure OpenAI Key
        string azureOpenAIKey = 
            ConsoleHelper.GetString($"Please insert your [yellow]{Statics.AzureOpenAIKey}[/] API key:");

        // Create OpenAI client
        client = 
            new(new Uri(endpoint), new AzureKeyCredential(azureOpenAIKey));

        // Get deployment name
        deploymentName = 
            ConsoleHelper.GetString("Please insert the [yellow]deployment name[/] of the model:");

        break;
}

if (client == null)
{
    return;
}

// Create header
ConsoleHelper.CreateHeader();

// Create ChatCompletionsOptions
ChatCompletionsOptions options = CreateChatCompletionsOptions();

// Chat implementation
while (true)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]Shakespeare:[/]");

    StringBuilder stringBuilder = new();
    await foreach (StreamingChatCompletionsUpdate? chatUpdate in await client.GetChatCompletionsStreamingAsync(options))
    {
        if (chatUpdate.ChoiceIndex.HasValue)
        {
            AnsiConsole.Markup($"[white]{chatUpdate.ContentUpdate}[/]");
            stringBuilder.Append(chatUpdate.ContentUpdate);
        }
    }

    AnsiConsole.WriteLine();
    options.Messages.Add(new ChatRequestAssistantMessage(stringBuilder.ToString()));

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]User:[/]");

    string? userMessage = Console.ReadLine();
    options.Messages.Add(new ChatRequestUserMessage(userMessage));
}


ChatCompletionsOptions CreateChatCompletionsOptions()
{
    ChatCompletionsOptions chatCompletionsOptions = new()
    {
        MaxTokens = 1000,
        Temperature = 0.7f,
        DeploymentName = deploymentName,
    };

    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage(
        "You are William Shakespeare, the English playwright, " +
        "poet and actor. Pretend to be William Shakespeare."));

    chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(
        "Please introduce yourself."));

    return chatCompletionsOptions;
}