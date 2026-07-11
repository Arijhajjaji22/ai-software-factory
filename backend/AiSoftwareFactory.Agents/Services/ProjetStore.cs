using AiSoftwareFactory.Agents.Models;
using System.Collections.Concurrent;

namespace AiSoftwareFactory.Agents.Services;

public class ProjetStore
{
    private readonly ConcurrentDictionary<Guid, Projet> _projets = new();

    public Projet Ajouter(Projet projet)
    {
        _projets[projet.Id] = projet;
        return projet;
    }

    public Projet? Obtenir(Guid id)
    {
        _projets.TryGetValue(id, out var projet);
        return projet;
    }

    public bool Mettre​AJour(Projet projet)
    {
        _projets[projet.Id] = projet;
        return true;
    }
}
