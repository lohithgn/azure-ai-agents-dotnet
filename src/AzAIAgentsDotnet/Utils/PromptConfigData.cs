using AzAIAgentsDotnet.Models;

namespace AzAIAgentsDotnet.Utils;

public static class PromptConfigData
{
    public static readonly Dictionary<string, PromptConfig> PromptConfigurations = new()
    {
        ["solveEquation"] = new PromptConfig
        {
            Prompt = "I need to solve the equation `3x + 11 = 14`. Can you help me?",
            Instructions = "You are a math agent. Use your knowledge to solve the equation.",
            Emoji = ":books:",
            Tool = null,
            Executor = null,
            FilePath = null,
            FileId = null
        },
        ["localCpusUsage"] = new PromptConfig
        {
            Prompt = "What is the average CPUs usage on my local machine?",
            Instructions = "You are a system administrator agent specializing in system performance and monitoring. Use the provided function to get the average CPU usage.",
            Emoji = "💾",
            Tool = ToolType.FunctionTool,
            Executor = null,
            FilePath = null,
            FileId = null
        },
        ["codeGenerator"] = new PromptConfig
        {
            Prompt = "Write a function that finds prime numbers",
            Instructions = "You are a math genius and a coding agent, specializing in assisting with code generation.",
            Emoji = "",
            Tool = ToolType.CodeInterpreter,
            Executor = null,
            FilePath = null,
            FileId = null
        },
        ["dataVisualization"] = new PromptConfig
        {
            Prompt = @"Create visualizations from the car_sales.csv data. Include charts for:
                            - Sales by Region 
                            - Relationships between Price, Mileage, and Year. 
                            - Sales by SalesPerson.
                            - Sales by Make, Model, and Year for 2023.",
            Instructions = "You are a data visualization agent. Use the remote code interpreter to analyze the data.",
            Emoji = "📊",
            Tool = ToolType.CodeInterpreter,
            FilePath = "./files/car_sales_data.csv",
            FileId = null,
            Executor = null
        },
        ["hotelReviews"] = new PromptConfig
        {
            Prompt = "Tell me about the hotel reviews in the hotel_reviews_data.csv.",
            Instructions = "You are a data analysis agent. Use the remote code interpreter to analyze the data.",
            Emoji = "🏨",
            Tool = ToolType.CodeInterpreter,
            FilePath = "./files/hotel_reviews_data.csv",
            FileId = "",
            Executor = null
        },
        ["insuranceCoverage"] = new PromptConfig
        {
            Prompt = "What are my health insurance plan coverage types?",
            Instructions = "You are a health insurance agent. Use the provided AI search tool to analyze the data.",
            Emoji = "🏥",
            Tool = ToolType.AiSearch,
            FilePath = null,
            FileId = null,
            Executor = null
        },
        ["stockMarket"] = new PromptConfig
        {
            Prompt = "What are the latest trends in the stock market?",
            Instructions = "You are a stock market analyst. Use the provided Bing Grounding tool to analyze the data.",
            Emoji = "📈",
            Tool = ToolType.BingGrounding,
            FilePath = null,
            FileId = null,
            Executor = null
        }
    };
}