using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceIngrediente : IServiceIngrediente
    {
        private readonly IRepositoryIngrediente _repository;

        public ServiceIngrediente(IRepositoryIngrediente repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<IngredienteDTO>> ListActivasAsync()
        {
            var ingredientes = await _repository.ListActivasAsync();

            return ingredientes
                .Select(i => new IngredienteDTO
                {
                    IngredienteID = i.IngredienteId,
                    Nombre = i.Nombre,
                    Activo = i.Activo
                })
                .ToList();
        }
    }
}
