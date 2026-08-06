using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;

namespace BrewTime.Infraestructure.Services
{
    public class HistorialNotificacionesEnMemoria : IHistorialNotificaciones
    {
        private readonly List<NotificacionEnviadaDTO> _notificaciones = new();
        private readonly object _lock = new();

        public void Registrar(NotificacionEnviadaDTO notificacion)
        {
            lock (_lock)
            {
                _notificaciones.Add(notificacion);
            }
        }

        public ICollection<NotificacionEnviadaDTO> ObtenerTodas()
        {
            lock (_lock)
            {
                return _notificaciones.OrderByDescending(n => n.FechaEnvio).ToList();
            }
        }
    }
}