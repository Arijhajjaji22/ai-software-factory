using Microsoft.EntityFrameworkCore;
using AiSoftwareFactory.Agents.Models;
using AiSoftwareFactory.Agents.Data;

namespace AiSoftwareFactory.Agents.Services;

public class ProjetStore
{
    private readonly AppDbContextPlateforme _db;

    public ProjetStore(AppDbContextPlateforme db)
    {
        _db = db;
    }

    public async Task AjouterAsync(Projet projet)
    {
        _db.Projets.Add(projet);
        await _db.SaveChangesAsync();
    }

    public async Task<Projet?> ObtenirAsync(Guid id)
    {
        return await _db.Projets
            .Include(p => p.Historique)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task MettreAJourAsync(Projet projet)
    {
        _db.Projets.Update(projet);
        await _db.SaveChangesAsync();
    }

    public async Task AjouterHistoriqueAsync(Guid projetId, EtapePipeline etape, string action, string? commentaire = null)
    {
        _db.HistoriqueValidations.Add(new HistoriqueValidation
        {
            ProjetId = projetId,
            Etape = etape,
            Action = action,
            Commentaire = commentaire
        });
        await _db.SaveChangesAsync();
    }
    public async Task<List<Projet>> ListerAsync()
    {
        return await _db.Projets
            .OrderByDescending(p => p.DateCreation)
            .ToListAsync();
    }
}