namespace GeneratedApp.Models
{
    public class Usager
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string Courriel { get; set; }
        public string PieceIdentite { get; set; }
        public string NumeroAbonne { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateInscription { get; set; }
    }
}
