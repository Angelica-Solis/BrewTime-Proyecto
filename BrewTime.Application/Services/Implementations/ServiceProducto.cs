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
        private readonly IRepositoryIngrediente _repositoryIngrediente;
        private readonly IMapper _mapper;

        public ServiceProducto(IRepositoryProducto repository, IRepositoryIngrediente repositoryIngrediente, IMapper mapper)
        {
            _repository = repository;
            _repositoryIngrediente = repositoryIngrediente;
            _mapper = mapper;
        }

        // Arma la colección de ingredientes del producto a partir de lo que llega del formulario:
        // - Los ya existentes se traen del catálogo (sin volver a insertarlos).
        // - Los nuevos se crean uno por uno (un INSERT independiente por cada ingrediente) y luego se asocian.
        private async Task AsignarIngredientesAsync(Producto entity, ProductoFormDTO dto)
        {
            if (dto.IngredientesSeleccionados != null && dto.IngredientesSeleccionados.Any())
            {
                var existentes = await _repositoryIngrediente.FindByIdsAsync(dto.IngredientesSeleccionados);
                foreach (var ingrediente in existentes)
                {
                    if (!entity.Ingrediente.Any(i => i.IngredienteId == ingrediente.IngredienteId))
                        entity.Ingrediente.Add(ingrediente);
                }
            }

            if (dto.IngredientesNuevos != null && dto.IngredientesNuevos.Any())
            {
                foreach (var nombre in dto.IngredientesNuevos.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    // Insert aparte por cada ingrediente nuevo
                    var ingrediente = await _repositoryIngrediente.ObtenerOCrearAsync(nombre);

                    if (!entity.Ingrediente.Any(i => i.IngredienteId == ingrediente.IngredienteId))
                        entity.Ingrediente.Add(ingrediente);
                }
            }
        }

        // Borra el archivo físico correspondiente a una imagen de producto.
        // rutaImagen viene como "/images/nombre.ext" (tal como se guarda en BD).
        private void EliminarArchivoFisico(string wwwRootPath, string rutaImagen)
        {
            if (string.IsNullOrWhiteSpace(rutaImagen)) return;

            var rutaRelativa = rutaImagen.TrimStart('/', '\\')
                                          .Replace('/', Path.DirectorySeparatorChar);
            var rutaFisica = Path.Combine(wwwRootPath, rutaRelativa);

            if (File.Exists(rutaFisica))
            {
                File.Delete(rutaFisica);
            }
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

            // Ingredientes: los existentes se asocian y los nuevos se crean uno por uno antes de guardar el producto
            await AsignarIngredientesAsync(entity, dto);

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

            // Ingredientes: se reconstruye la relación completa según lo que llegó del formulario
            // (los checkboxes desmarcados se quitan, los marcados/nuevos quedan asociados)
            entity.Ingrediente.Clear();
            await AsignarIngredientesAsync(entity, dto);

            // Eliminar imágenes marcadas: se borra tanto el registro de BD
            // como el archivo físico en wwwroot/images para no dejar basura.
            if (imagenesAEliminar.Any())
            {
                foreach (var imagenId in imagenesAEliminar)
                {
                    var imagen = entity.ProductoImagen
                        .FirstOrDefault(i => i.ImagenId == imagenId);

                    if (imagen != null)
                    {
                        EliminarArchivoFisico(wwwRootPath, imagen.RutaImagen);
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