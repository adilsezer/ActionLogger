using System;

namespace ActionLogger.Models
{
    public class UserAction
    {
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
    }
}
