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
    public class ServiceEstacionCocina : IServiceEstacionCocina
    {
        private readonly IRepositoryEstacionCocina _repository;
        private readonly IMapper _mapper;

        public ServiceEstacionCocina(IRepositoryEstacionCocina repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ICollection<EstacionProcesoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<EstacionProcesoDTO>>(list);
        }
    }
}
