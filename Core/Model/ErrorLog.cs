using System;

namespace Core.Model
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? Path { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
