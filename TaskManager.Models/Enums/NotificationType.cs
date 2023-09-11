namespace TaskManager.Models.Enums
{
    public enum NotificationType
    {
        DueDateReminder,
        StatusUpdate
    }

    public static class NotificationTypeExtension
    {
        public static string? GetStringValue(this NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.DueDateReminder => "DueDateReminder",
                NotificationType.StatusUpdate => "StatusUpdate",
                _ => null
            };
        }
    }
}
