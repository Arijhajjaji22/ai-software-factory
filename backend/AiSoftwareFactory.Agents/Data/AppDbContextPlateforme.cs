using Microsoft.EntityFrameworkCore;
using AiSoftwareFactory.Agents.Models;
using System.Text.Json;

namespace AiSoftwareFactory.Agents.Data;

public class AppDbContextPlateforme : DbContext
{
	public AppDbContextPlateforme(DbContextOptions<AppDbContextPlateforme> options) : base(options) { }

	public DbSet<Projet> Projets => Set<Projet>();
	public DbSet<HistoriqueValidation> HistoriqueValidations => Set<HistoriqueValidation>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var jsonOptions = new JsonSerializerOptions();

		modelBuilder.Entity<Projet>(entite =>
		{
			entite.Property(p => p.ResultatAnalyse)
				.HasConversion(
					v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
					v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<AnalyseResult>(v, jsonOptions))
				.HasColumnType("nvarchar(max)");

			entite.Property(p => p.ResultatArchitecture)
				.HasConversion(
					v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
					v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<ArchitectureResult>(v, jsonOptions))
				.HasColumnType("nvarchar(max)");

			entite.Property(p => p.ResultatDevBackend)
				.HasConversion(
					v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
					v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<DevBackendResult>(v, jsonOptions))
				.HasColumnType("nvarchar(max)");

			entite.Property(p => p.ResultatDevFrontend)
				.HasConversion(
					v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
					v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<DevBackendResult>(v, jsonOptions))
				.HasColumnType("nvarchar(max)");

			entite.Property(p => p.ResultatDevOps)
				.HasConversion(
					v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
					v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<DevBackendResult>(v, jsonOptions))
				.HasColumnType("nvarchar(max)");

			entite.HasMany(p => p.Historique)
				.WithOne()
				.HasForeignKey(h => h.ProjetId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}
}