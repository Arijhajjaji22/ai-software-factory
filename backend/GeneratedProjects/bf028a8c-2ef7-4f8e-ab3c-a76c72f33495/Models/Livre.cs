using System;

namespace GeneratedApp.Models
{
    public class Livre
    {
        public Guid Id { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }
        public int NombreTotalExemplaires { get; set; }
        public int NombreExemplairesDisponibles { get; set; }
    }
}