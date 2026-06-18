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

        public async Task<ProcesoPreparacionDetalleDTO> DetailByProductoAsync(int productoId)
        {
            var lista = await _repository.FindByProductoIdAsync(productoId);

            if (lista == null || !lista.Any())
                return null;

            return new ProcesoPreparacionDetalleDTO
            {
                NombreProducto = lista.First().Producto.Nombre,
                Procesos = _mapper.Map<ICollection<ProcesoPreparacionDTO>>(lista)
            };
        }

        public async Task<ICollection<ProcesoPreparacionListadoDTO>>ListadoProcesosAsync()
        {
            var lista = await _repository.ListAsync();

            return lista
                .GroupBy(x => x.ProductoId)
                .Select(g => new ProcesoPreparacionListadoDTO
                {
                    ProductoId = g.Key,
                    NombreProducto = g.First().Producto.Nombre,
                    CantidadProcesos = g.Count()
                })
                .ToList();
        }
    }
}
