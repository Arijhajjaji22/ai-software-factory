using System;

namespace GeneratedApp.Models
{
    public class PreferenceNotification
    {
        public Guid Id { get; set; }
        public Guid UsagerId { get; set; }
        public bool CanalEmail { get; set; }
        public bool CanalSMS { get; set; }
        public bool CanalPush { get; set; }
        public bool RappelRetour { get; set; }
        public string CategoriesPref { get; set; }
        public string Frequence { get; set; }
    }
}
