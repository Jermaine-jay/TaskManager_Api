using TaskManager.Services.Interfaces;

namespace TaskManager.Api.Extensions
{
    public class ConsumeScopedServiceHostedService : BackgroundService
    {
        private readonly ILogger<ConsumeScopedServiceHostedService> _logger;

        private readonly IServiceProvider _serviceProvider;

        public ConsumeScopedServiceHostedService(IServiceProvider serviceProvider,
            ILogger<ConsumeScopedServiceHostedService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<INotificationService>();

            }
            //await _serviceProvider.GetService<INotificationServiceFactory>().Create().CreateReminderNotification(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
