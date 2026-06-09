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
    public class ServiceMenuDiaSemana : IServiceMenuDiaSemana
    {
        private readonly IRepositoryMenuDiaSemana _repository;
        private readonly IMapper _mapper;

        public ServiceMenuDiaSemana(IRepositoryMenuDiaSemana repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuDiaSemanaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuDiaSemanaDTO>>(list);
        }

        public async Task<MenuDiaSemanaDTO> FindByIdAsync(int menuId, byte diaSemana)
        {
            var entity = await _repository.FindByIdAsync(menuId, diaSemana);
            return _mapper.Map<MenuDiaSemanaDTO>(entity);
        }
    }
}
