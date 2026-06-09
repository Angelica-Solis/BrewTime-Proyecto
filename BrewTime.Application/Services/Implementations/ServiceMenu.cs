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
    public class ServiceMenu : IServiceMenu
    {
        private readonly IRepositoryMenu _repository;
        private readonly IMapper _mapper;

        public ServiceMenu(IRepositoryMenu repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuDTO>>(list);
        }

        public async Task<MenuDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<MenuDTO>(entity);
        }
    }
}
