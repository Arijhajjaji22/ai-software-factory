using System;

namespace GeneratedApp.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid LivreId { get; set; }
        public Guid UsagerId { get; set; }
        public DateTime DateReservation { get; set; }
        public int Position { get; set; }
        public DateTime DateExpiration { get; set; }
        public bool Notified { get; set; }
    }
}
