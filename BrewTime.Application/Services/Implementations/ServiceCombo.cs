using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceCombo : IServiceCombo
    {
        private readonly IRepositoryCombo _repository;
        private readonly IMapper _mapper;

        public ServiceCombo(IRepositoryCombo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<ComboDTO>> ListAsync()
        {
            // Obtener datos del repositorio
            var list = await _repository.ListAsync();

            // Mapear List<Combo> a ICollection<ComboDTO>
            var collection = _mapper.Map<ICollection<ComboDTO>>(list);

            // Retornar lista
            return collection;
        }

        public async Task<ComboDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ComboDetalleDTO>(@object);
            return objectMapped;
        }
    }
}