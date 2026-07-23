namespace AiSoftwareFactory.Agents.Models;

public enum EtapePipeline
{
    Analyse,
    Architecture,
    DevBackend,
    DevFrontend,
    DevOps,
    Termine
}

public enum StatutEtape
{
    EnAttenteDeValidation,
    Valide,
    Rejete
}

public class Projet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IdeeMetier { get; set; } = string.Empty;
    public EtapePipeline EtapeActuelle { get; set; }
    public StatutEtape StatutEtapeActuelle { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public AnalyseResult? ResultatAnalyse { get; set; }
    public ArchitectureResult? ResultatArchitecture { get; set; }
    public DevBackendResult? ResultatDevBackend { get; set; }
    public DevBackendResult? ResultatDevFrontend { get; set; }
    public DevBackendResult? ResultatDevOps { get; set; }

    public List<HistoriqueValidation> Historique { get; set; } = new();
}