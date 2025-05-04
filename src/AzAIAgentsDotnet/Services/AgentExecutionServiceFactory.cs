using AzAIAgentsDotnet.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AzAIAgentsDotnet.Services;

internal class AgentExecutionServiceFactory(IServiceProvider serviceProvider) 
    : IAgentExecutionServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IAgentExecutionService GetAgentService(PromptConfig promptConfig)
    {
        return promptConfig.Tool switch
        {
            null => _serviceProvider.GetRequiredService<NoToolAgentService>(),
            ToolType.FunctionTool => _serviceProvider.GetRequiredService<FunctionToolAgentService>(),
            ToolType.CodeInterpreter => _serviceProvider.GetRequiredService<CodeInterpreterAgentService>(),
            ToolType.AiSearch => _serviceProvider.GetRequiredService<AiSearchAgentService>(),
            ToolType.BingGrounding => _serviceProvider.GetRequiredService<BingGroundingAgentService>(),
            _ => throw new NotSupportedException("Unsupported tool type.")
        };
    }
}
