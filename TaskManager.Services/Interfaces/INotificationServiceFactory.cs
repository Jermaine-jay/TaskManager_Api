namespace TaskManager.Services.Interfaces
{
    public interface INotificationServiceFactory
    {
        Task<object> Create();
    }
}
