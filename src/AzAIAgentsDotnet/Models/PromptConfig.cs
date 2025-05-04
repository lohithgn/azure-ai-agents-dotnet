using Azure.AI.Projects;

namespace AzAIAgentsDotnet.Models;

public class PromptConfig
{
    public string Prompt { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public string? Emoji { get; set; }
    public ToolType? Tool { get; set; }
    public string? FilePath { get; set; }
    public string? FileId { get; set; }
    public bool? AiSearch { get; set; }
    public object? Executor { get; set; }
    public List<ToolDefinition>? Tools { get; set; }
    public ToolResources? ToolResources { get; set; }
}

public enum ToolType
{
    CodeInterpreter,
    FunctionTool,
    AiSearch,
    BingGrounding
}