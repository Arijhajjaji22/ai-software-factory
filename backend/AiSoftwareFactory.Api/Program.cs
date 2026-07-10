using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/ping-ia", async (IConfiguration config) =>
{
    var apiKey = config["Gemini:ApiKey"];

    var kernel = Kernel.CreateBuilder()
        .AddGoogleAIGeminiChatCompletion("gemini-2.5-flash-lite", apiKey!)
        .Build();

    var result = await kernel.InvokePromptAsync(
        "Réponds uniquement par la phrase : Bonjour, je suis connecté à Gemini !");

    return Results.Ok(result.ToString());
});

app.Run();