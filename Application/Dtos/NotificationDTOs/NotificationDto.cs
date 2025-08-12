using System;

namespace DTOs.NotificationDTOs
{
    /// <summary>
    /// Lightweight DTO used internally to return a newly-created notification.
    /// The system does not currently expose a public API to list notifications,
    /// but several services/interfaces depend on this type for mapping.
    /// </summary>
    public class NotificationDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int LanguageId { get; set; }
        public bool IsSent { get; set; }
        public Guid? UserId { get; set; }
    }
}
