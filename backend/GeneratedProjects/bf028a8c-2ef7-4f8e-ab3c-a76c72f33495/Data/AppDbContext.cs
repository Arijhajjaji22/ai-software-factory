using Microsoft.EntityFrameworkCore;
using GeneratedApp.Models;

namespace GeneratedApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Livre> Livres { get; set; }
        public DbSet<Membre> Membres { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }
    }
}