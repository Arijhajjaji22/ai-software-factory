using System;

namespace GeneratedApp.Models
{
    public class Livre
    {
        public Guid Id { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public string ISBN { get; set; }
        public string Categorie { get; set; }
        public int StockTotal { get; set; }
        public int StockDisponible { get; set; }
    }
}