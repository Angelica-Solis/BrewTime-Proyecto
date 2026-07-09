using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IMapper _mapper;

        public ServiceProducto(IRepositoryProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ── Lectura ──────────────────────────────────────────

        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }

        public async Task<ICollection<ProductoDTO>> ListInactivosAsync()
        {
            var list = await _repository.ListInactivosAsync();
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }

        public async Task<ProductoDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            return _mapper.Map<ProductoDetalleDTO>(@object);
        }

        public async Task<ProductoFormDTO> FindFormByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            return _mapper.Map<ProductoFormDTO>(@object);
        }

        // ── Escritura ─────────────────────────────────────────

        public async Task CreateAsync(ProductoFormDTO dto, string wwwRootPath)
        {
            var entity = _mapper.Map<Producto>(dto);

            // Guardar primero para obtener el ProductoId generado
            await _repository.CreateAsync(entity);

            // Procesar imágenes nuevas
            if (dto.ImagenesNuevas != null && dto.ImagenesNuevas.Any())
            {
                var carpeta = Path.Combine(wwwRootPath, "images");
                Directory.CreateDirectory(carpeta);

                bool esPrimera = true;
                foreach (var archivo in dto.ImagenesNuevas)
                {
                    if (archivo.Length > 0)
                    {
                        var nombreArchivo = $"{entity.ProductoId}_{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
                        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                        using var stream = new FileStream(rutaFisica, FileMode.Create);
                        await archivo.CopyToAsync(stream);

                        entity.ProductoImagen.Add(new ProductoImagen
                        {
                            ProductoId = entity.ProductoId,
                            RutaImagen = $"/images/{nombreArchivo}",
                            EsPrincipal = esPrimera,
                            FechaSubida = DateTime.Now
                        });
                        esPrimera = false;
                    }
                }

                await _repository.UpdateAsync(entity);
            }
        }

        public async Task UpdateAsync(ProductoFormDTO dto, string wwwRootPath)
        {
            // Obtener entidad original con imágenes incluidas
            var entity = await _repository.FindByIdAsync(dto.ProductoID);
            var imagenesAEliminar = dto.ImagenesAEliminar ?? new List<int>();

            // Patrón del profe: source → destination sobre el objeto existente 
            _mapper.Map(dto, entity);

            // Eliminar imágenes marcadas (borrado lógico de archivo físico no aplica,
            // pero sí quitamos el registro de BD si el usuario lo marcó)
            if (imagenesAEliminar.Any())
            {
                foreach (var imagenId in imagenesAEliminar)
                {
                    var imagen = entity.ProductoImagen
                        .FirstOrDefault(i => i.ImagenId == imagenId);

                    if (imagen != null)
                    {
                        _repository.DeleteImagen(imagen);
                    }
                }
            }

            // Agregar imágenes nuevas
            if (dto.ImagenesNuevas != null && dto.ImagenesNuevas.Any())
            {
                var carpeta = Path.Combine(wwwRootPath, "images");
                Directory.CreateDirectory(carpeta);

                bool hayPrincipal = entity.ProductoImagen
                    .Any(i => i.EsPrincipal && !imagenesAEliminar.Contains(i.ImagenId));
                foreach (var archivo in dto.ImagenesNuevas)
                {
                    if (archivo.Length > 0)
                    {
                        var nombreArchivo = $"{entity.ProductoId}_{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
                        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                        using var stream = new FileStream(rutaFisica, FileMode.Create);
                        await archivo.CopyToAsync(stream);

                        entity.ProductoImagen.Add(new ProductoImagen
                        {
                            ProductoId = entity.ProductoId,
                            RutaImagen = $"/images/{nombreArchivo}",
                            EsPrincipal = !hayPrincipal,
                            FechaSubida = DateTime.Now
                        });
                        hayPrincipal = true;
                    }
                }
            }

            await _repository.UpdateAsync(entity);
        }

        public async Task ToggleActivoAsync(int id)
        {
            await _repository.ToggleActivoAsync(id);
        }

        public async Task<ICollection<ProductoDTO>> ProductosSinProcesoAsync()
        {
            var productos = await _repository.ProductosSinProcesoAsync();

            return _mapper.Map<ICollection<ProductoDTO>>(productos);
        }
    }
}
