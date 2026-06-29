using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceCategoria : IServiceCategoria
    {
        private readonly IRepositoryCategoria _repository;

        public ServiceCategoria(IRepositoryCategoria repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<CategoriaDTO>> ListActivasAsync()
        {
            var categorias = await _repository.ListActivasAsync();

            return categorias
                .Select(c => new CategoriaDTO
                {
                    CategoriaID = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activo = c.Activo
                })
                .ToList();
        }
    }
}
