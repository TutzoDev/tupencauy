using Microsoft.EntityFrameworkCore;
using tupencauy.Data;

namespace tupencauywebapi.SignalR.Servicios
{
    public interface IAvisoService
    {
        Task VerificarYEnviarAvisos();
        Task VerificarYEnviarResultados();
        Task EnviarAvisoResumenPosicionesYFuturosCruces();
    }

    public class AvisoService : IAvisoService
    {
        private readonly IServiceProvider _serviceProvider;

        public AvisoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task VerificarYEnviarAvisos()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

                // Obtener eventos deportivos que están próximos y aún no han ocurrido
                var eventosProximos = await dbContext.EventosDeportivos
                    .Where(e => e.FechaInicio > DateTime.Now && e.FechaInicio <= DateTime.Now.AddDays(1))
                    .ToListAsync();

                foreach (var evento in eventosProximos)
                {
                    // Verificar para cada evento si los usuarios necesitan ser notificados
                    var usuariosSinPrediccion = await dbContext.PencaSitioUsuario
                        .Include(psu => psu.Predicciones)
                        .Where(psu => psu.Predicciones.All(p => p.IdUnoVsUno != evento.Id))
                        .ToListAsync();

                    foreach (var usuario in usuariosSinPrediccion)
                    {
                        // Verificar si ya se ha enviado una notificación para este usuario y evento
                        var notificacionExistente = await dbContext.Notificaciones
                            .AnyAsync(n => n.UsuarioId == usuario.IdUsuario && n.Mensaje.Contains(evento.Nombre));

                        if (!notificacionExistente)
                        {
                            // Obtener el pencaSitio asociado al usuario
                            var pencaSitio = await dbContext.PencasSitio
                                .FirstOrDefaultAsync(ps => ps.Id == usuario.IdPencaSitio);

                            if (pencaSitio != null)
                            {
                                // Obtener el sitio asociado al pencaSitio
                                var sitio = await dbContext.Sitios
                                    .FirstOrDefaultAsync(s => s.TenantId == pencaSitio.SitioTenantId);

                                if (sitio != null)
                                {
                                    var mensaje = $"¡Recuerda hacer tu predicción para el evento {evento.Nombre} que ocurrirá el {evento.FechaInicio}!";
                                    await notificacionService.CrearNotificacion(usuario.IdUsuario, mensaje, sitio.Id);
                                }
                            }
                        }
                    }
                }
            }
        }
        public async Task VerificarYEnviarResultados()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

                // Obtener eventos UnoVsUno que ya han ocurrido pero cuyos resultados aún no han sido notificados
                var eventosConResultados = await dbContext.UnoVsUno
                    .Where(e => e.FechaFin <= DateTime.Now && e.ScoreUno != null && e.ScoreDos != null && !e.ResultadoNotificado)
                    .ToListAsync();

                foreach (var evento in eventosConResultados)
                {
                    // Obtener los usuarios que participaron en las predicciones del evento
                    var usuarios = await dbContext.PencaSitioUsuario
                        .Include(psu => psu.Predicciones)
                        .Where(psu => psu.Predicciones.Any(p => p.IdUnoVsUno == evento.Id))
                        .ToListAsync();

                    foreach (var usuario in usuarios)
                    {
                        // Obtener el pencaSitio asociado al usuario
                        var pencaSitio = await dbContext.PencasSitio
                            .FirstOrDefaultAsync(ps => ps.Id == usuario.IdPencaSitio);

                        if (pencaSitio != null)
                        {
                            // Obtener el sitio asociado al pencaSitio
                            var sitio = await dbContext.Sitios
                                .FirstOrDefaultAsync(s => s.TenantId == pencaSitio.SitioTenantId);

                            if (sitio != null)
                            {
                                var mensaje = $"El resultado del evento {evento.Nombre} es: {evento.ScoreUno} - {evento.ScoreDos}";
                                await notificacionService.CrearNotificacion(usuario.IdUsuario, mensaje, sitio.Id);
                            }
                        }
                    }

                    // Marcar el resultado del evento como notificado
                    evento.ResultadoNotificado = true;
                    dbContext.UnoVsUno.Update(evento);
                }

                await dbContext.SaveChangesAsync();
            }
        }
        public async Task EnviarAvisoResumenPosicionesYFuturosCruces()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

                // Lógica para enviar aviso de resumen de posiciones
                var pencasActivas = await dbContext.Pencas
                    .Include(p => p.EventosDeportivos)
                    .Where(p => p.FechaFin >= DateTime.Today)
                    .ToListAsync();

                foreach (var penca in pencasActivas)
                {
                    // Obtener el PencaSitio asociado
                    var pencaSitio = await dbContext.PencasSitio
                        .FirstOrDefaultAsync(ps => ps.PencaId == penca.Id);

                    if (pencaSitio != null)
                    {
                        // Obtener el Sitio asociado al PencaSitio
                        var sitio = await dbContext.Sitios
                            .FirstOrDefaultAsync(s => s.TenantId == pencaSitio.SitioTenantId);

                        if (sitio != null)
                        {
                            var mensajePosiciones = $"El resumen de posiciones para la penca '{penca.Nombre}' está listo. Revisa tu posición actual.";
                            await notificacionService.CrearNotificacion(penca.Id, mensajePosiciones, sitio.Id);

                            // Lógica para enviar aviso de futuros cruces
                            var eventosProximos = await dbContext.EventosDeportivos
                                .Where(e => e.FechaInicio > DateTime.Now && e.FechaInicio <= DateTime.Now.AddDays(7))
                                .ToListAsync();

                            foreach (var evento in eventosProximos)
                            {
                                var mensajeCruces = $"Se aproxima el evento '{evento.Nombre}'. Prepárate para hacer tus predicciones.";
                                await notificacionService.CrearNotificacion(penca.Id, mensajeCruces, sitio.Id);
                            }
                        }
                    }
                }
            }
        }
    }
}
