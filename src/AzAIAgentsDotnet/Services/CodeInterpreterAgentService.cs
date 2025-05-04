using AzAIAgentsDotnet.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;

namespace AzAIAgentsDotnet.Services;

internal class CodeInterpreterAgentService(AgentsClient agentsClient, IConfiguration configuration) 
    : BaseAgentService(agentsClient, configuration)
{
    protected override string GetAgentName() => "my-code-interpreter-agent";

    protected override List<ToolDefinition>? GetTools(PromptConfig promptConfig)
    {
        return [new CodeInterpreterToolDefinition()];
    }

    protected override IEnumerable<MessageAttachment>? GetAttachments(PromptConfig promptConfig)
    {
        if (!string.IsNullOrEmpty(promptConfig.FileId))
        {
            return [new MessageAttachment(promptConfig.FileId, GetTools(promptConfig))];
        }
        return null;
    }

    protected override async Task PreExecutionAsync(PromptConfig promptConfig)
    {
        if (!string.IsNullOrEmpty(promptConfig.FilePath))
        {
            var file = new FileInfo(promptConfig.FilePath);
            if (file.Exists)
            {
                var uploadResponse = await _agentsClient.UploadFileAsync(
                    filePath: promptConfig.FilePath,
                    purpose: AgentFilePurpose.Agents);
                var uploadedFile = uploadResponse.Value;

                promptConfig.FileId = uploadedFile.Id;
            }
        }
    }
}