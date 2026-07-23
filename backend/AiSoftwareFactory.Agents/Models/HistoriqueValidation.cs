using System;

namespace AiSoftwareFactory.Agents.Models;

public class HistoriqueValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjetId { get; set; }
    public EtapePipeline Etape { get; set; }
    public string Action { get; set; } = string.Empty; // "Validé", "Rejeté", "Régénéré"
    public string? Commentaire { get; set; }
    public DateTime DateAction { get; set; } = DateTime.UtcNow;
}