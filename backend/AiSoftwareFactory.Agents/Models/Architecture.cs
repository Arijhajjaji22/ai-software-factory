namespace AiSoftwareFactory.Agents.Models;

public class Attribut
{
    public string Nom { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class Entite
{
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Attribut> Attributs { get; set; } = new();
}

public class Relation
{
    public string EntiteSource { get; set; } = string.Empty;
    public string EntiteCible { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ArchitectureResult
{
    public List<Entite> Entites { get; set; } = new();
    public List<Relation> Relations { get; set; } = new();
}