using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.ChatCompletion;
using AiSoftwareFactory.Agents.Personas;
using AiSoftwareFactory.Agents.Models;
using AiSoftwareFactory.Agents.Services;
using System.Text.Json;
using AiSoftwareFactory.Agents.Data;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContextPlateforme>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Plateforme")));

builder.Services.AddScoped<ProjetStore>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
var app = builder.Build();
app.UseCors("AllowAngularDev");
// Fonction utilitaire réutilisable : construit le kernel connecté à Gemini
#pragma warning disable SKEXP0010
Kernel CreerKernel(IConfiguration config)
{
    var apiKey = config["Groq:ApiKey"];
    return Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(
            modelId: "openai/gpt-oss-120b",
            endpoint: new Uri("https://api.groq.com/openai/v1"),
            apiKey: apiKey!)
        .Build();
}
#pragma warning restore SKEXP0010
OpenAIPromptExecutionSettings CreerSettings(int maxTokens = 4096) => new()
{
    MaxTokens = maxTokens
};

async Task<string> AppelerAgentAvecRetry(IChatCompletionService chatService, ChatHistory chatHistory, PromptExecutionSettings settings)
{
    int delaiSecondes = 20;

    for (int tentative = 1; tentative <= 3; tentative++)
    {
        try
        {
            var reponse = await chatService.GetChatMessageContentAsync(chatHistory, settings);
            return reponse.ToString() ?? string.Empty;
        }
        catch (Microsoft.SemanticKernel.HttpOperationException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            if (tentative == 3)
                throw;

            await Task.Delay(TimeSpan.FromSeconds(delaiSecondes));
            delaiSecondes *= 2;
        }
    }

    throw new InvalidOperationException("Échec après plusieurs tentatives.");
}
string ExtraireJson(string texte)
{
    int debut = texte.IndexOf('{');
    int fin = texte.LastIndexOf('}');
    if (debut == -1 || fin == -1 || fin < debut)
        return texte;
    return texte.Substring(debut, fin - debut + 1);
}
string NormaliserContenu(string contenu)
{
    return contenu
        .Replace("\\r\\n", "\n")
        .Replace("\\n", "\n")
        .Replace("\\\"", "\"");
}

// 1. Créer un projet et lancer l'agent Analyste
app.MapPost("/api/projets", async (IdeeRequest requete, IConfiguration config, ProjetStore store) =>
{
    var kernel = CreerKernel(config);

    var chatHistory = new ChatHistory(AnalystePersona.SystemPrompt);
    chatHistory.AddUserMessage(requete.Idee);

    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings());
    var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var resultat = JsonSerializer.Deserialize<AnalyseResult>(jsonBrut, options);

    var projet = new Projet
    {
        IdeeMetier = requete.Idee,
        EtapeActuelle = EtapePipeline.Analyse,
        StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation,
        ResultatAnalyse = resultat
    };

    await store.AjouterAsync(projet);

    return Results.Ok(projet);
});

// 2. Consulter l'état d'un projet
app.MapGet("/api/projets/{id}", async (Guid id, ProjetStore store) =>
{
    var projet = await store.ObtenirAsync(id);
    return projet is null ? Results.NotFound() : Results.Ok(projet);
});

// 3. Valider l'étape actuelle, avancer le pipeline, et déclencher l'agent suivant
app.MapPost("/api/projets/{id}/valider", async (Guid id, ProjetStore store, IConfiguration config) =>
{
    var projet = await store.ObtenirAsync(id);
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

    // Déclenche automatiquement l'agent correspondant à la nouvelle étape
    if (projet.EtapeActuelle == EtapePipeline.Architecture)
    {
        var kernel = CreerKernel(config);
        var chatHistory = new ChatHistory(ArchitectePersona.SystemPrompt);
        chatHistory.AddUserMessage(JsonSerializer.Serialize(projet.ResultatAnalyse));

        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings());
        var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        projet.ResultatArchitecture = JsonSerializer.Deserialize<ArchitectureResult>(jsonBrut, options);

        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;
    }
    else if (projet.EtapeActuelle == EtapePipeline.DevBackend)
    {
        var fichiersGeneres = new List<FichierGenere>();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var entite in projet.ResultatArchitecture!.Entites)
        {
            var kernel = CreerKernel(config);
            var chatHistory = new ChatHistory(DevBackendPersona.SystemPrompt);
            chatHistory.AddUserMessage(
                "Génère UNIQUEMENT le modèle et le contrôleur pour cette entité (pas le DbContext) : "
                + JsonSerializer.Serialize(entite));

            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings(6000));
            var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
            var resultatUnEntite = JsonSerializer.Deserialize<DevBackendResult>(jsonBrut, options);

            if (resultatUnEntite is not null)
                fichiersGeneres.AddRange(resultatUnEntite.Fichiers);
        }

        // Génération séparée du DbContext, qui a besoin de connaître TOUTES les entités
        var kernelDbContext = CreerKernel(config);
        var chatHistoryDbContext = new ChatHistory(
            "Tu es un développeur backend C#. Génère UNIQUEMENT le fichier AppDbContext.cs " +
            "(namespace GeneratedApp.Data), avec un DbSet pour chaque entité listée. " +
            "Réponds en JSON strict : { \"fichiers\": [{ \"cheminRelatif\": \"Data/AppDbContext.cs\", \"contenu\": \"...\" }] }");
        chatHistoryDbContext.AddUserMessage(JsonSerializer.Serialize(projet.ResultatArchitecture));

        var chatServiceDbContext = kernelDbContext.GetRequiredService<IChatCompletionService>();
        var reponseDbContext = await AppelerAgentAvecRetry(chatServiceDbContext, chatHistoryDbContext, CreerSettings(2000));
        var jsonDbContext = ExtraireJson(reponseDbContext.Replace("```json", "").Replace("```", "").Trim());
        var resultatDbContext = JsonSerializer.Deserialize<DevBackendResult>(jsonDbContext, options);

        if (resultatDbContext is not null)
            fichiersGeneres.AddRange(resultatDbContext.Fichiers);

        projet.ResultatDevBackend = new DevBackendResult { Fichiers = fichiersGeneres };

        var dossierProjet = Path.Combine(app.Environment.ContentRootPath, "..", "GeneratedProjects", projet.Id.ToString());
        Directory.CreateDirectory(dossierProjet);

        foreach (var fichier in fichiersGeneres)
        {
            var cheminComplet = Path.Combine(dossierProjet, fichier.CheminRelatif);
            var dossierFichier = Path.GetDirectoryName(cheminComplet);
            if (dossierFichier is not null)
                Directory.CreateDirectory(dossierFichier);

            await File.WriteAllTextAsync(cheminComplet, NormaliserContenu(fichier.Contenu));
        }

        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;
    }
    else if (projet.EtapeActuelle == EtapePipeline.DevFrontend)
    {
        var fichiersGeneres = new List<FichierGenere>();

        foreach (var entite in projet.ResultatArchitecture!.Entites)
        {
            try
            {
                var kernel = CreerKernel(config);
                var chatHistory = new ChatHistory(DevFrontendPersona.SystemPrompt);
                chatHistory.AddUserMessage(
                    "Génère le composant liste UNIQUEMENT pour cette entité : " + JsonSerializer.Serialize(entite));

                var chatService = kernel.GetRequiredService<IChatCompletionService>();
                var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings(6000));
                var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultatUnEntite = JsonSerializer.Deserialize<DevBackendResult>(jsonBrut, options);

                if (resultatUnEntite is not null)
                    fichiersGeneres.AddRange(resultatUnEntite.Fichiers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DevFrontend] Échec génération pour l'entité '{entite.Nom}' : {ex.Message}");
                // on continue avec les autres entités plutôt que de tout faire planter
            }
        }

        projet.ResultatDevFrontend = new DevBackendResult { Fichiers = fichiersGeneres };

        var dossierProjet = Path.Combine(app.Environment.ContentRootPath, "..", "GeneratedProjects", projet.Id.ToString(), "frontend");
        Directory.CreateDirectory(dossierProjet);

        foreach (var fichier in fichiersGeneres)
        {
            var cheminComplet = Path.Combine(dossierProjet, fichier.CheminRelatif);
            var dossierFichier = Path.GetDirectoryName(cheminComplet);
            if (dossierFichier is not null)
                Directory.CreateDirectory(dossierFichier);

            await File.WriteAllTextAsync(cheminComplet, NormaliserContenu(fichier.Contenu));
        }

        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;
    }
    else if (projet.EtapeActuelle == EtapePipeline.DevOps)
    {
        var kernel = CreerKernel(config);
        var chatHistory = new ChatHistory(DevOpsPersona.SystemPrompt);
        chatHistory.AddUserMessage(
            "Architecture du projet : " + JsonSerializer.Serialize(projet.ResultatArchitecture));

        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings(4000));
        var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var resultatDevOps = JsonSerializer.Deserialize<DevBackendResult>(jsonBrut, options);

        projet.ResultatDevOps = resultatDevOps;

        if (resultatDevOps is not null)
        {
            var dossierProjet = Path.Combine(app.Environment.ContentRootPath, "..", "GeneratedProjects", projet.Id.ToString());
            Directory.CreateDirectory(dossierProjet);

            foreach (var fichier in resultatDevOps.Fichiers)
            {
                var cheminComplet = Path.Combine(dossierProjet, fichier.CheminRelatif);
                var dossierFichier = Path.GetDirectoryName(cheminComplet);
                if (dossierFichier is not null)
                    Directory.CreateDirectory(dossierFichier);

                await File.WriteAllTextAsync(cheminComplet, NormaliserContenu(fichier.Contenu));
            }
        }

        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;
    }
    else if (projet.EtapeActuelle != EtapePipeline.Termine)
    {
        projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;
    }

    await store.MettreAJourAsync(projet);

    return Results.Ok(projet);
});

// 4. Rejeter le résultat actuel et demander à l'IA de régénérer (avec un commentaire optionnel)
app.MapPost("/api/projets/{id}/rejeter", async (Guid id, RejetRequest requete, ProjetStore store, IConfiguration config) =>
{
    var projet = await store.ObtenirAsync(id);
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
    var reponseTexte = await AppelerAgentAvecRetry(chatService, chatHistory, CreerSettings(5000));
    var jsonBrut = ExtraireJson(reponseTexte.Replace("```json", "").Replace("```", "").Trim());
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    projet.ResultatAnalyse = JsonSerializer.Deserialize<AnalyseResult>(jsonBrut, options);
    projet.StatutEtapeActuelle = StatutEtape.EnAttenteDeValidation;

    await store.MettreAJourAsync(projet);
    return Results.Ok(projet);
});

// 5. Modifier directement le résultat (édition manuelle, sans repasser par l'IA)
app.MapPut("/api/projets/{id}/analyse", async (Guid id, AnalyseResult nouveauResultat, ProjetStore store) =>
{
    var projet = await store.ObtenirAsync(id);
    if (projet is null)
        return Results.NotFound();

    projet.ResultatAnalyse = nouveauResultat;
    await store.MettreAJourAsync(projet);

    return Results.Ok(projet);
});
// 6. Télécharger le projet généré sous forme de zip
app.MapGet("/api/projets/{id}/telecharger", async (Guid id, ProjetStore store) =>
{
    var projet = await store.ObtenirAsync(id);
    if (projet is null)
        return Results.NotFound();

    var dossierProjet = Path.Combine(app.Environment.ContentRootPath, "..", "GeneratedProjects", projet.Id.ToString());
    if (!Directory.Exists(dossierProjet))
        return Results.NotFound("Aucun fichier généré pour ce projet.");

    var cheminZip = Path.Combine(Path.GetTempPath(), $"{projet.Id}.zip");
    if (File.Exists(cheminZip))
        File.Delete(cheminZip);

    ZipFile.CreateFromDirectory(dossierProjet, cheminZip);

    var octets = await File.ReadAllBytesAsync(cheminZip);
    File.Delete(cheminZip);

    return Results.File(octets, "application/zip", $"projet-{projet.Id}.zip");
});
// 2bis. Lister tous les projets (résumé, sans les résultats complets)
app.MapGet("/api/projets", async (ProjetStore store) =>
{
    var projets = await store.ListerAsync();
    return Results.Ok(projets);
});
app.Run();

public record IdeeRequest(string Idee);
public record RejetRequest(string? Commentaire);