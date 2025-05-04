using AzAIAgentsDotnet.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;

namespace AzAIAgentsDotnet.Services;

internal class AiSearchAgentService(AIProjectClient projectClient, AgentsClient agentsClient, IConfiguration configuration) : BaseAgentService(agentsClient, configuration)
{
    private readonly AIProjectClient _projectClient = projectClient;

    protected override string GetAgentName() => "my-ai-search-agent";

    protected override List<ToolDefinition>? GetTools(PromptConfig promptConfig)
    {
        return [new AzureAISearchToolDefinition()];
    }

    protected override async Task<ToolResources?> GetToolResources(PromptConfig promptConfig)
    {
        ConnectionResponse connection = await _projectClient.GetConnectionsClient()
                                                            .GetConnectionAsync(_configuration["AI_SEARCH_CONNECTION_NAME"]) 
                                                            ?? throw new InvalidOperationException("No connections found for the Azure AI Search.");
        AzureAISearchResource searchResource = new();
        searchResource.IndexList.Add(new AISearchIndexResource(connection.Id, _configuration["AI_SEARCH_INDEX_NAME"])
        {
            QueryType = AzureAISearchQueryType.VectorSimpleHybrid
        });
        ToolResources toolResource = new()
        {
            AzureAISearch = searchResource
        };
        return toolResource;
    }
}