using AzAIAgentsDotnet.Models;
using AzAIAgentsDotnet.Tools;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;

namespace AzAIAgentsDotnet.Services;

internal class FunctionToolAgentService(AgentsClient agentsClient, IConfiguration configuration) 
    : BaseAgentService(agentsClient, configuration)
{

    readonly string _toolName = "getCpuUsage";

    protected override string GetAgentName() => "my-functool-agent";

    protected override List<ToolDefinition>? GetTools(PromptConfig promptConfig)
    {
        FunctionToolDefinition getCpuUsageTool = new(_toolName, "Gets the current CPU usage of the system.");
        return [getCpuUsageTool];
    }

    protected override ToolOutput? GetResolvedToolOutput(RequiredToolCall toolCall)
    {
        if (toolCall is RequiredFunctionToolCall functionToolCall)
        {
            if (functionToolCall.Name == _toolName)
            {
                return new ToolOutput(toolCall, FunctionToolFactory.GetCpuUsage());
            }
        }
        return null;
    }


}
