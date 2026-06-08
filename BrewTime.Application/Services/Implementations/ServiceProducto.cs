using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
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
        public async Task<ProductoDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ProductoDetalleDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            // Obtener datos del repositorio
            var list = await _repository.ListAsync();

            // Mapear List<Producto> a ICollection<ProductoDTO>
            var collection = _mapper.Map<ICollection<ProductoDTO>>(list);

            // Retornar lista
            return collection;
        }

    }
}
