using AzAIAgentsDotnet.Services;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzAIAgentsDotnet.DependencyInjecion;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfiureServices(this IServiceCollection services)
    {
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddSingleton(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>()["PROJECT_CONNECTION_STRING"];
            return new AIProjectClient(connectionString, new DefaultAzureCredential());
        });

        services.AddSingleton(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>()["PROJECT_CONNECTION_STRING"];
            return new AgentsClient(connectionString, new DefaultAzureCredential());
        });

        services.AddSingleton<IAgentExecutionServiceFactory, AgentExecutionServiceFactory>();
        services.AddTransient<NoToolAgentService>();
        services.AddTransient<FunctionToolAgentService>();
        services.AddTransient<CodeInterpreterAgentService>();
        services.AddTransient<AiSearchAgentService>();
        services.AddTransient<BingGroundingAgentService>();

        return services;
    }
}
