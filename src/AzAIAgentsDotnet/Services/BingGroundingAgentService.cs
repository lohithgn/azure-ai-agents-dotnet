using AzAIAgentsDotnet.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzAIAgentsDotnet.Services;

internal class BingGroundingAgentService : BaseAgentService
{
    private readonly AIProjectClient _projectCliet;
    private readonly string _bingGroundingConnectionName;


    public BingGroundingAgentService(AIProjectClient projectCliet, AgentsClient agentsClient, IConfiguration configuration)
        : base(agentsClient, configuration)
    {
        _projectCliet = projectCliet;
        _bingGroundingConnectionName = _configuration["BING_GROUNDING_CONNECTION_ID"]!;
    }
    protected override string GetAgentName() => "my-bing-grounding-agent";
    protected override List<ToolDefinition>? GetTools(PromptConfig promptConfig)
    {
        ConnectionResponse bingConnection = _projectCliet.GetConnectionsClient().GetConnection(_bingGroundingConnectionName);
        var connectionId = bingConnection.Id;

        ToolConnectionList connectionList = new()
        {
            ConnectionList = { new ToolConnection(connectionId) }
        };
        BingGroundingToolDefinition bingGroundingTool = new(connectionList);

        return [bingGroundingTool];
    }
}
