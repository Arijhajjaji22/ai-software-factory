using System;

namespace GeneratedApp.Models
{
    public class NotificationLog
    {
        public Guid Id { get; set; }
        public Guid UsagerId { get; set; }
        public string Type { get; set; }
        public DateTime DateEnvoi { get; set; }
        public string Details { get; set; }
    }
}