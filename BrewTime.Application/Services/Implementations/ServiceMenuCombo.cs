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
    public class ServiceMenuCombo : IServiceMenuCombo
    {
        private readonly IRepositoryMenuCombo _repository;
        private readonly IMapper _mapper;

        public ServiceMenuCombo(IRepositoryMenuCombo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuComboDTO>>(list);
        }

        public async Task<MenuComboDTO> FindByIdAsync(int menuId, int comboId)
        {
            var entity = await _repository.FindByIdAsync(menuId, comboId);
            return _mapper.Map<MenuComboDTO>(entity);
        }
    }
}
