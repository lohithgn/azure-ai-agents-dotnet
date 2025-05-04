using AzAIAgentsDotnet.Models;

namespace AzAIAgentsDotnet.Services
{
    internal interface IAgentExecutionServiceFactory
    {
        IAgentExecutionService GetAgentService(PromptConfig promptConfig);
    }
}