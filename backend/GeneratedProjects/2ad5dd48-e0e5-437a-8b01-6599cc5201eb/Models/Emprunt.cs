using System;

namespace GeneratedApp.Models
{
    public class Emprunt
    {
        public Guid Id { get; set; }
        public Guid ExemplaireId { get; set; }
        public Guid UsagerId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFinPrevue { get; set; }
        public DateTime DateRetour { get; set; }
        public decimal FraisRetard { get; set; }
    }
}
