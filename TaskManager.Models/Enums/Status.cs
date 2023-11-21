namespace TaskManager.Models.Enums
{
    public enum Status : int
    {
        Pending =1,
        InProgress,
        Completed
    }

    public static class StatusTypeExtension
    {
        public static string? GetStringValue(this Status status)
        {
            return status switch
            {
                Status.Pending => "Pending",
                Status.InProgress => "InProgress",
                Status.Completed => "Completed",
                _ => null
            };
        }
    }
}
