using BrewTime.Application.Services.Interfaces;

namespace BrewTime.Web.Jobs
{
    public class CombosInconsistentesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CombosInconsistentesBackgroundService> _logger;

        public CombosInconsistentesBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CombosInconsistentesBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var serviceCombo = scope.ServiceProvider.GetRequiredService<IServiceCombo>();
                    var seEjecutoTarea = await serviceCombo.RevisarYNotificarProductosInconsistentesAsync();

                    _logger.LogInformation(seEjecutoTarea
                        ? "Tarea ejecutada: se detectaron y notificaron productos inconsistentes en combos."
                        : "Tarea ejecutada: no se encontraron inconsistencias.");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}