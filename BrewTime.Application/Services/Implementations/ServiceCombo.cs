using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceCombo : IServiceCombo
    {
        private readonly IRepositoryCombo _repository;
        private readonly IMapper _mapper;
        private readonly IServiceCorreo _serviceCorreo;
        private readonly IHistorialNotificaciones _historialNotificaciones;

        public ServiceCombo(IRepositoryCombo repository, IMapper mapper, IServiceCorreo serviceCorreo, IHistorialNotificaciones historialNotificaciones)
        {
            _repository = repository;
            _mapper = mapper;
            _serviceCorreo = serviceCorreo;
            _historialNotificaciones = historialNotificaciones;
        }

        // ── Lectura ──────────────────────────────────────────

        public async Task<ICollection<ComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        public async Task<ICollection<ComboDTO>> ListInactivosAsync()
        {
            var list = await _repository.ListInactivosAsync();
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        public async Task<ComboDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            return _mapper.Map<ComboDetalleDTO>(@object);
        }

        public async Task<ComboFormDTO> FindFormByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            return _mapper.Map<ComboFormDTO>(@object);
        }

        // ── Escritura ─────────────────────────────────────────

        public async Task CreateAsync(ComboFormDTO dto)
        {
            // Mapear DTO → entidad base
            var entity = _mapper.Map<Combo>(dto);

            // Construir las relaciones ComboProducto desde los seleccionados
            entity.ComboProducto = dto.ProductosSeleccionados
                .Where(p => p.Seleccionado && p.Cantidad > 0)
                .Select(p => new ComboProducto
                {
                    ProductoId = p.ProductoID,
                    Cantidad = p.Cantidad
                })
                .ToList();

            await _repository.CreateAsync(entity);
        }

        public async Task UpdateAsync(ComboFormDTO dto)
        {
            // Obtener entidad original con sus productos (patrón del profe)
            var entity = await _repository.FindByIdAsync(dto.ComboID);

            // Mapear cambios del formulario sobre la entidad existente
            _mapper.Map(dto, entity);

            // Reconstruir la colección de productos desde cero
            entity.ComboProducto.Clear();
            foreach (var p in dto.ProductosSeleccionados.Where(p => p.Seleccionado && p.Cantidad > 0))
            {
                entity.ComboProducto.Add(new ComboProducto
                {
                    ComboId = entity.ComboId,
                    ProductoId = p.ProductoID,
                    Cantidad = p.Cantidad
                });
            }

            await _repository.UpdateAsync(entity);
        }

        public async Task ToggleActivoAsync(int id)
        {
            await _repository.ToggleActivoAsync(id);
        }

        public async Task<ICollection<ProductoInconsistenteEnComboDTO>> ObtenerProductosInconsistentesAsync()
        {
            var combos = await _repository.ListActivosConProductosEIngredientesAsync();
            var resultado = new List<ProductoInconsistenteEnComboDTO>();

            foreach (var combo in combos)
            {
                foreach (var comboProducto in combo.ComboProducto)
                {
                    var producto = comboProducto.Producto;

                    if (!producto.Activo)
                    {
                        resultado.Add(new ProductoInconsistenteEnComboDTO
                        {
                            ComboID = combo.ComboId,
                            NombreCombo = combo.Nombre,
                            ProductoID = producto.ProductoId,
                            NombreProducto = producto.Nombre,
                            MotivoInconsistencia = "Producto inactivo"
                        });
                    }
                    else if (!producto.Ingrediente.Any())
                    {
                        resultado.Add(new ProductoInconsistenteEnComboDTO
                        {
                            ComboID = combo.ComboId,
                            NombreCombo = combo.Nombre,
                            ProductoID = producto.ProductoId,
                            NombreProducto = producto.Nombre,
                            MotivoInconsistencia = "Producto sin ingredientes"
                        });
                    }
                }
            }

            return resultado;
        }

        public async Task<bool> RevisarYNotificarProductosInconsistentesAsync()
        {
            // CONDICIÓN: consulta la BD para decidir si la tarea debe actuar
            var inconsistencias = await ObtenerProductosInconsistentesAsync();

            if (!inconsistencias.Any())
                return false;

            // TAREA: enviar la notificación
            var cuerpo = ConstruirCuerpoCorreo(inconsistencias);
            var asunto = "Combos con productos inconsistentes - BrewTime";

            await _serviceCorreo.EnviarAsync("fabzamoramendez13@gmail.com", asunto, cuerpo);

            _historialNotificaciones.Registrar(new NotificacionEnviadaDTO
            {
                FechaEnvio = DateTime.Now,
                Asunto = asunto,
                Detalle = cuerpo
            });

            return true;
        }

        private static string ConstruirCuerpoCorreo(ICollection<ProductoInconsistenteEnComboDTO> items)
        {
            var sb = new System.Text.StringBuilder("<h3>Combos con productos inconsistentes:</h3><ul>");
            foreach (var i in items)
                sb.Append($"<li><b>{i.NombreCombo}</b> — Producto: {i.NombreProducto} — Motivo: {i.MotivoInconsistencia}</li>");
            sb.Append("</ul>");
            return sb.ToString();
        }
    }
}