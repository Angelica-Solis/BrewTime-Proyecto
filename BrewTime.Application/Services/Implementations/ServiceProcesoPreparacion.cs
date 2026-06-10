using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceProcesoPreparacion: IServiceProcesoPreparacion
    {
        private readonly IRepositoryProcesoPreparacion _repository;
        private readonly IMapper _mapper;

        public ServiceProcesoPreparacion(IRepositoryProcesoPreparacion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        async Task<ICollection<ProcesoPreparacionDTO>> IServiceProcesoPreparacion.ListAsync()
        {
            // Obtener datos del repositorio
            var list = await _repository.ListAsync();

            // Mapear List<Combo> a ICollection<ComboDTO>
            var collection = _mapper.Map<ICollection<ProcesoPreparacionDTO>>(list);

            // Retornar lista
            return collection;
        }

        async Task<ProcesoPreparacionDTO> IServiceProcesoPreparacion.FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<ProcesoPreparacionDTO>(@object);
            return objectMapped;
        }
    }
}
