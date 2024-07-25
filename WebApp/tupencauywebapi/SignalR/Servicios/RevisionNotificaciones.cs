
namespace tupencauywebapi.SignalR.Servicios
{
    public class RevisionNotificaciones : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RevisionNotificaciones(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var avisoService = scope.ServiceProvider.GetRequiredService<IAvisoService>();
                        await avisoService.VerificarYEnviarAvisos();
                        await avisoService.VerificarYEnviarResultados();
                        await avisoService.EnviarAvisoResumenPosicionesYFuturosCruces();
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones según sea necesario
                    // Aquí puedes registrar la excepción en un log, enviar una notificación, etc.
                    Console.WriteLine($"Error en el servicio de avisos: {ex.Message}");
                }

                // Intervalo de tiempo entre cada ejecución del servicio
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // Cambia el intervalo según tus necesidades
            }
        }
    }
}
