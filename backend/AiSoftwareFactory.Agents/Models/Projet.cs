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
    public EtapePipeline EtapeActuelle { get; set; } = EtapePipeline.Analyse;
    public StatutEtape StatutEtapeActuelle { get; set; } = StatutEtape.EnAttenteDeValidation;
    public AnalyseResult? ResultatAnalyse { get; set; }
}