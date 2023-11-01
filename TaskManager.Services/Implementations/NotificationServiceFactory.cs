using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services.Interfaces;


namespace TaskManager.Services.Implementations
{
    public class NotificationServiceFactory : INotificationServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public NotificationServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public INotificationService Create()
        {
            return _serviceProvider.GetService<INotificationService>();
        }
    }
}
