using AzAIAgentsDotnet.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Linq;

namespace AzAIAgentsDotnet.Services;

internal abstract class BaseAgentService(AgentsClient agentsClient, IConfiguration configuration) 
    : IAgentExecutionService
{
    protected readonly AgentsClient _agentsClient = agentsClient;
    protected readonly IConfiguration _configuration = configuration;
    public event EventHandler<string>? ProgressUpdated; // Event declaration

    protected virtual Task PreExecutionAsync(PromptConfig promptConfig)
    {
        // Default implementation does nothing
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync(PromptConfig promptConfig)
    {
        // Step 1: Pre-execution tasks (e.g., file upload)
        OnProgressUpdated("Preparing agent...");
        await PreExecutionAsync(promptConfig);

        // Step 2: Create the agent
        OnProgressUpdated("Creating agent...");
        var agent = await CreateAgentAsync(promptConfig);

        // Step 3: Create a thread and send the message
        OnProgressUpdated("Creating thread and sending message...");
        var thread = await CreateThreadAndSendMessageAsync(promptConfig, agent);

        // Step 4: Run the agent
        OnProgressUpdated("Running agent...");
        await RunAgentAsync(thread.Id, agent.Id);

        // Step 5: Process the agent's response
        OnProgressUpdated("Processing agent response...");
        await ProcessAgentResponseAsync(thread.Id);

        // Step 6: Cleanup
        OnProgressUpdated("Cleaning up...");
        await CleanupAsync(thread.Id, agent.Id);
    }

    protected virtual async Task<Agent> CreateAgentAsync(PromptConfig promptConfig)
    {
        var modelName = _configuration["MODEL_DEPLOYMENT_NAME"];
        var agentName = GetAgentName();
        var tools = GetTools(promptConfig);
        var toolResources = await GetToolResources(promptConfig)!;

        var agentResponse = await _agentsClient.CreateAgentAsync(
            model: modelName,
            name: agentName,
            instructions: promptConfig.Instructions ?? "You are a helpful agent.",
            tools: tools,
            toolResources: toolResources);
        return agentResponse.Value;
    }

    protected virtual async Task<AgentThread> CreateThreadAndSendMessageAsync(PromptConfig promptConfig, Agent agent)
    {
        var thread = (await _agentsClient.CreateThreadAsync()).Value;

        await _agentsClient.CreateMessageAsync(
            thread.Id,
            MessageRole.User,
            promptConfig.Prompt,
            attachments: GetAttachments(promptConfig));

        return thread;
    }

    protected virtual async Task RunAgentAsync(string threadId, string agentId)
    {
        var runResponse = await _agentsClient.CreateRunAsync(threadId, agentId);
        var run = runResponse.Value;

        do
        {
            await Task.Delay(500);
            runResponse = await _agentsClient.GetRunAsync(threadId, run.Id);

            if (runResponse.Value.Status == RunStatus.RequiresAction
                && runResponse.Value.RequiredAction is SubmitToolOutputsAction submitToolOutputsAction)
            {
                List<ToolOutput> toolOutputs = [];
                foreach (RequiredToolCall toolCall in submitToolOutputsAction.ToolCalls)
                {
                    toolOutputs.Add(GetResolvedToolOutput(toolCall)!);
                }
                runResponse = await _agentsClient.SubmitToolOutputsToRunAsync(runResponse.Value, toolOutputs);
            }

        } while (runResponse.Value.Status == RunStatus.Queued || runResponse.Value.Status == RunStatus.InProgress);
    }

    protected virtual async Task ProcessAgentResponseAsync(string threadId)
    {
        var afterRunMessagesResponse = await _agentsClient.GetMessagesAsync(threadId);
        var messages = afterRunMessagesResponse.Value.Data.Take(afterRunMessagesResponse.Value.Data.Count-1);
        foreach(var message in messages)
        {
            foreach (MessageContent contentItem in message!.ContentItems)
            {
                if (contentItem is MessageTextContent textItem)
                {
                    AnsiConsole.WriteLine(textItem.Text);
                    AnsiConsole.WriteLine();
                }
                else if (contentItem is MessageImageFileContent imageFileItem)
                {
                    var fileNameResponse = await _agentsClient.GetFileAsync(imageFileItem.FileId);
                    var fileName = fileNameResponse.Value.Filename;
                    var fileContentResponse = await _agentsClient.GetFileContentAsync(imageFileItem.FileId);
                    var fileContent = fileContentResponse.Value;
                    // Ensure the "downloads" directory exists
                    var downloadsDirectory = Path.Combine(".", "downloads");
                    if (!Directory.Exists(downloadsDirectory))
                    {
                        Directory.CreateDirectory(downloadsDirectory);
                    }

                    // Save the file to the "downloads" directory
                    var filePath = Path.Combine(downloadsDirectory, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileContent.ToStream().CopyToAsync(fileStream);
                    }

                    AnsiConsole.WriteLine($"Saved image file to: {fileName}");
                }
            }
        }
    }

    protected virtual async Task CleanupAsync(string threadId, string agentId)
    {
        await _agentsClient.DeleteThreadAsync(threadId);
        await _agentsClient.DeleteAgentAsync(agentId);
    }

    // Abstract or virtual methods for customization
    protected abstract string GetAgentName();
    protected virtual List<ToolDefinition>? GetTools(PromptConfig promptConfig) => null;
    protected virtual Task<ToolResources?> GetToolResources(PromptConfig promptConfig)
    {
        return Task.FromResult<ToolResources?>(null);
    }
    protected virtual IEnumerable<MessageAttachment>? GetAttachments(PromptConfig promptConfig) => null;
    protected virtual ToolOutput? GetResolvedToolOutput(RequiredToolCall toolCall) => null;

    protected void OnProgressUpdated(string message)
    {
        ProgressUpdated?.Invoke(this, message);
    }
}