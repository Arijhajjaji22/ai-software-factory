namespace AiSoftwareFactory.Agents.Models;

public class UserStory
{
    public string Titre { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Benefice { get; set; } = string.Empty;
    public List<string> CriteresAcceptation { get; set; } = new();
}

public class AnalyseResult
{
    public List<UserStory> UserStories { get; set; } = new();
}