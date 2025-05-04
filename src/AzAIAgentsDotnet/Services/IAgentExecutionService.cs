using AzAIAgentsDotnet.Models;

namespace AzAIAgentsDotnet.Services;

public interface IAgentExecutionService
{
    Task ExecuteAsync(PromptConfig promptConfig);
    
    event EventHandler<string> ProgressUpdated;
}