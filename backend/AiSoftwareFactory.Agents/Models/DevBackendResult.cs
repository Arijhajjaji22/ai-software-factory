namespace AiSoftwareFactory.Agents.Models;

public class FichierGenere
{
    public string CheminRelatif { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
}

public class DevBackendResult
{
    public List<FichierGenere> Fichiers { get; set; } = new();
}