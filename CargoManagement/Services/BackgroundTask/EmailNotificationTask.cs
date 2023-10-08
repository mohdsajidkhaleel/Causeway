
using CargoManagement.Services.Abstractions;

namespace CargoManagement.Services.BackgroundTask
{
    public class EmailNotificationTask : BackgroundService
    {
        public readonly IServiceProvider serviceProvider;
        public EmailNotificationTask(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var _service = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();
                    await _service.CheckNewBookings();
                }
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}
