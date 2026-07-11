using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.ChatCompletion;
using AiSoftwareFactory.Agents.Personas;
using AiSoftwareFactory.Agents.Models;
using AiSoftwareFactory.Agents.Services;
using System.Text.Json;
using Microsoft.SemanticKernel.Connectors.Google.Core;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProjetStore>();

var app = builder.Build();

// Fonction utilitaire réutilisable : construit le kernel connecté à Gemini
Kernel CreerKernel(IConfiguration config)
{
    var apiKey = config["Gemini:ApiKey"];
    return Kernel.CreateBuilder()
        .AddGoogleAIGeminiChatCompletion("gemini-2.5-flash-lite", apiKey!)
        .Build();
}
GeminiPromptExecutionSettings CreerSettings() => new()
{
    MaxTokens = 4096
};
// 1. Créer un projet et lancer l'agent Analyste
app.MapPost("/api/projets", async (IdeeRequest requete, IConfiguration config, ProjetStore store) =>
{
    var kernel = CreerKernel(config);

    var chatHistory = new ChatHistory(AnalystePersona.SystemPrompt);
    chatHistory.AddUserMessage(requete.Idee);

    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    var reponse = await chatService.GetChatMessageContentAsync(chatHistory, CreerSettings());
    var jsonBrut = reponse.ToString().Replace("```json", "").Replace("```", "").Trim();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var resultat = JsonSerializer.Deserialize<AnalyseResult>(jsonBrut, options);

    var projet = new Projet
    {
        IdeeMetier = requete.Idee,
        EtapeActuelle = EtapePipeline.Analyse,
        StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation,
        ResultatAnalyse = resultat
    };

    store.Ajouter(projet);

    return Results.Ok(projet);
});

// 2. Consulter l'état d'un projet
app.MapGet("/api/projets/{id}", (Guid id, ProjetStore store) =>
{
    var projet = store.Obtenir(id);
    return projet is null ? Results.NotFound() : Results.Ok(projet);
});
// 3. Valider l'étape actuelle et avancer le pipeline
app.MapPost("/api/projets/{id}/valider", (Guid id, ProjetStore store) =>
{
    var projet = store.Obtenir(id);
    if (projet is null)
        return Results.NotFound();

    if (projet.StatutEtapeActuelle != StatutEtape.EnAttenteDeValidation)
        return Results.BadRequest("Ce projet n'est pas en attente de validation.");

    projet.StatutEtapeActuelle = StatutEtape.Valide;

    projet.EtapeActuelle = projet.EtapeActuelle switch
    {
        EtapePipeline.Analyse => EtapePipeline.Architecture,
        EtapePipeline.Architecture => EtapePipeline.DevBackend,
        EtapePipeline.DevBackend => EtapePipeline.DevFrontend,
        EtapePipeline.DevFrontend => EtapePipeline.DevOps,
        EtapePipeline.DevOps => EtapePipeline.Termine,
        _ => projet.EtapeActuelle
    };

    if (projet.EtapeActuelle != EtapePipeline.Termine)
        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;

    store.MettreAJour(projet);

    return Results.Ok(projet);
});
// 4. Rejeter le résultat actuel et demander à l'IA de régénérer (avec un commentaire optionnel)
app.MapPost("/api/projets/{id}/rejeter", async (Guid id, RejetRequest requete, ProjetStore store, IConfiguration config) =>
{
    var projet = store.Obtenir(id);
    if (projet is null)
        return Results.NotFound();

    if (projet.EtapeActuelle != EtapePipeline.Analyse)
        return Results.BadRequest("Le rejet n'est actuellement supporté que pour l'étape Analyse.");

    var kernel = CreerKernel(config);
    var chatHistory = new ChatHistory(AnalystePersona.SystemPrompt);
    chatHistory.AddUserMessage(projet.IdeeMetier);
    chatHistory.AddAssistantMessage(JsonSerializer.Serialize(projet.ResultatAnalyse));
    chatHistory.AddUserMessage(
        $"Ce résultat ne convient pas. Voici le retour de l'utilisateur : \"{requete.Commentaire}\". " +
        "Génère une nouvelle version qui tient compte de ce retour, en respectant le même format JSON.");

    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    var reponse = await chatService.GetChatMessageContentAsync(chatHistory);

    var jsonBrut = reponse.ToString().Replace("```json", "").Replace("```", "").Trim();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    projet.ResultatAnalyse = JsonSerializer.Deserialize<AnalyseResult>(jsonBrut, options);
    projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;

    store.MettreAJour(projet);
    return Results.Ok(projet);
});

// 5. Modifier directement le résultat (édition manuelle, sans repasser par l'IA)
app.MapPut("/api/projets/{id}/analyse", (Guid id, AnalyseResult nouveauResultat, ProjetStore store) =>
{
    var projet = store.Obtenir(id);
    if (projet is null)
        return Results.NotFound();
    
    projet.ResultatAnalyse = nouveauResultat;
    store.MettreAJour(projet);

    return Results.Ok(projet);
});
app.Run();

public record IdeeRequest(string Idee);
public record RejetRequest(string? Commentaire);