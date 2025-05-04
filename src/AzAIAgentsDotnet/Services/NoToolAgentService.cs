using AzAIAgentsDotnet.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace AzAIAgentsDotnet.Services;

internal class NoToolAgentService : BaseAgentService
{
    
    public NoToolAgentService(AgentsClient agentsClient, IConfiguration configuration)
        : base(agentsClient, configuration)
    {
    }

    protected override string GetAgentName() => "my-no-tool-agent";
}