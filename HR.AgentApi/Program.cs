using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();


// ----------------------------------------
// 1. Read Ollama configuration
// ----------------------------------------

var ollamaEndpoint =
    builder.Configuration["Ollama:Endpoint"]
    ?? "http://localhost:11434";

var modelName =
    builder.Configuration["Ollama:Model"]
    ?? "llama3.2:1b";


// ----------------------------------------
// 2. Connect to MCP Server
// ----------------------------------------

var mcpEndpoint =
    builder.Configuration["Mcp:Endpoint"]
    ?? "http://localhost:3001/mcp";

var transport = new HttpClientTransport(
    new HttpClientTransportOptions
    {
        Endpoint = new Uri(mcpEndpoint)
    });

await using var mcpClient =
    await McpClient.CreateAsync(transport);


// ----------------------------------------
// 3. Discover available MCP tools
// ----------------------------------------

var mcpTools = await mcpClient.ListToolsAsync();

Console.WriteLine("Discovered MCP tools:");

foreach (var tool in mcpTools)
{
    Console.WriteLine(
        $"{tool.Name} - {tool.Description}");
}


// ----------------------------------------
// 4. Create Ollama Chat Client
// ----------------------------------------

var chatClient = new OllamaApiClient(
    new Uri(ollamaEndpoint),
    modelName);


// ----------------------------------------
// 5. Create Microsoft AI Agent
// ----------------------------------------

AIAgent agent = chatClient.AsAIAgent(
    name: "HRAssistant",

    instructions: """
        You are an intelligent HR assistant.

        You have access to these tools:

        1. GetEmployee
           Gets employee details using employee ID.

        2. GetLeaveBalance
           Gets employee leave balance.

        3. GetLeavePolicy
           Gets company leave policy.

        Rules:

        - Use the MCP tools to retrieve actual data.
        - Never invent employee information.
        - Never invent leave balances.
        - If employee ID is missing, ask the user.
        - You can use multiple tools when required.
        - Explain the result in simple English.
        """,

    tools: mcpTools.Cast<AITool>().ToArray()
);


// ----------------------------------------
// 6. Create Chat API
// ----------------------------------------

app.MapPost("/api/chat", async (ChatRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(
            new { Error = "Message is required" });
    }

    var result = await agent.RunAsync(request.Message);

    return Results.Ok(new
    {
        Response = result.Text
    });
});

app.MapGet("/", () =>
    "HR AI Agent is running");


// ----------------------------------------
// 7. Start API
// ----------------------------------------

app.Run("http://localhost:5000");


public class ChatRequest
{
    public string Message { get; set; } = "";
}