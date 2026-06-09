using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceMenuProducto : IServiceMenuProducto
    {
        private readonly IRepositoryMenuProducto _repository;
        private readonly IMapper _mapper;

        public ServiceMenuProducto(IRepositoryMenuProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuProductoDTO>>(list);
        }

        public async Task<MenuProductoDTO> FindByIdAsync(int menuId, int productoId)
        {
            var entity = await _repository.FindByIdAsync(menuId, productoId);
            return _mapper.Map<MenuProductoDTO>(entity);
        }
    }
}
