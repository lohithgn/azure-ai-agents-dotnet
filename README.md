# Azure AI Agent Service Demo

This demo originally started out using the code from the [Azure AI Agent QuickStart](https://learn.microsoft.com/azure/ai-services/agents/quickstart), but has expanded to show different agent tooling features:

- Perform a simple calculation (no tools)
- Make function calls using Function Calling Tools
- Create a function using code interpreter
- Examine a CSV file and create diagrams from it using code interpreter
- Examine a CSV file and provide analysis using code interpreter
- Perform RAG functionality using AI search
- Use real-time public web data using Bing Grounding
- Return usage stats about in/out tokens

![Demo Prompts](./images/prompts.png)

To use the demo you'll need to complete the steps in the [QuickStart](https://learn.microsoft.com/azure/ai-services/agents/quickstart) to set up your Azure AI Foundry project. If you'd like to use the AI Search/RAG functionality in the demo, you'll find details about the setup in the [AI Search tooling](https://learn.microsoft.com/azure/ai-services/agents/how-to/tools/azure-ai-search?tabs=azurecli%2Cjavascript&pivots=code-examples) document.

## Running the Demo

After going through the QuickStart steps (and optionally the AI Search and Bing Grounding tooling setup), perform the following steps:

1. Open `src/AzAiAgentsDotnet` > `appsettings.json`. 
```json
{
  "PROJECT_CONNECTION_STRING": "",
  "MODEL_DEPLOYMENT_NAME": "",
  "EMBEDDING_MODEL_DEPLOYMENT_NAME": "",
  "BING_GROUNDING_CONNECTION_ID": "",
  "AI_SEARCH_CONNECTION_NAME": "",
  "AI_SEARCH_INDEX_NAME": ""
}
```

2. Update `appsettings.json` with your configuration values.

2. Assign your Azure AI Foundry connection string, your AI Search and Bing Grounding connection name from Azure AI Foundry, and (optionally) the model deployment name in the appropriate sections of the configuration file.

3. Open a terminal and navigate to `src\AzAIAgentsDotnet`.
    ```bash
    cd src\AzAIAgentsDotnet
    ```

4. Restore the project dependencies:

    ```bash
    dotnet restore
    ```

4. Run the demo:
    ```bash
    dotnet run
    ```

## Prompts Demo Videos:

### Prompt 1 Demo
<video src="./images/prompt1.mp4" controls></video>

### Prompt 2 Demo
<video src="./images/prompt2.mp4" controls></video>


### Prompt 3 Demo
<video src="./images/prompt3.mp4" controls></video>


### Prompt 4 Demo
<video src="./images/prompt4.mp4" controls></video>


### Prompt 5 Demo
<video src="./images/prompt5.mp4" controls></video>


### Prompt 7 Demo
<video src="./images/prompt7.mp4" controls></video>
