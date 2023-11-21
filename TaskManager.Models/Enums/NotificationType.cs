namespace TaskManager.Models.Enums
{
    public enum NotificationType
    {
        DueDateReminder,
        StatusUpdate,
        PriorityUpdate
    }

    public static class NotificationTypeExtension
    {
        public static string? GetStringValue(this NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.DueDateReminder => "DueDateReminder",
                NotificationType.StatusUpdate => "StatusUpdate",
                NotificationType.PriorityUpdate => "PriorityUpdate",
                _ => null
            };
        }
    }
}
