using System;

namespace GeneratedApp.Models
{
    public class Emprunt
    {
        public Guid Id { get; set; }
        public Guid LivreId { get; set; }
        public Guid MembreId { get; set; }
        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetourEffective { get; set; }
        public bool EstEnRetard { get; set; }
    }
}