using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
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
        // de lectura
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
        public async Task<ProcesoPreparacionFormDTO> FindFormByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            return _mapper.Map<ProcesoPreparacionFormDTO>(@object);
        }

        // de accion 

        public async Task CreateAsync(ProcesoPreparacionFormDTO dto)
        {
            // obtener solo las estaciones que seleccione el usuario
            var estacionesSeleccionadas = dto.Estaciones
                .Where(x => x.Seleccionada)
                .ToList();

            // se debio seleccionar al menos 1 estacion
            if (!estacionesSeleccionadas.Any())
            {
                throw new Exception("Debe seleccionar al menos una estación.");
            }

            // vlida orden
            foreach (var estacion in estacionesSeleccionadas)
            {
                if (estacion.Orden <= 0)
                {
                    throw new Exception($"Debe indicar un orden válido para la estación '{estacion.Nombre}'.");
                }

                if (estacion.TiempoEstimadoMin <= 0)
                {
                    throw new Exception($"Debe indicar un tiempo válido para la estación '{estacion.Nombre}'.");
                }
            }

            // valida que no existan ordenes repetidas
            if (estacionesSeleccionadas
                .GroupBy(x => x.Orden)
                .Any(g => g.Count() > 1))
            {
                throw new Exception("No pueden existir dos estaciones con el mismo número de orden.");
            }

            // crea entidades
            var procesos = estacionesSeleccionadas
                .Select(x => new ProcesoPreparacion
                {
                    ProductoId = dto.ProductoId,
                    EstacionId = x.EstacionId,
                    Orden = x.Orden,
                    TiempoEstimadoMin = x.TiempoEstimadoMin
                })
                .ToList();

            await _repository.CreateRangeAsync(procesos);
        }

        public async Task UpdateAsync(ProcesoPreparacionFormDTO dto)
        {
            var procesosActuales = await _repository.FindByProductoIdAsync(dto.ProductoId);

            foreach (var proceso in procesosActuales)
            {
                var estacionDTO = dto.Estaciones
                    .FirstOrDefault(e => e.EstacionId == proceso.EstacionId);

                if (estacionDTO != null)
                {
                    _mapper.Map(estacionDTO, proceso);

                    await _repository.UpdateAsync(proceso);
                }
            }

            foreach (var estacionDTO in dto.Estaciones)
            {
                var existe = procesosActuales.Any(
                    p => p.EstacionId == estacionDTO.EstacionId);

                if (!existe)
                {
                    var nuevoProceso = _mapper.Map<ProcesoPreparacion>(estacionDTO);
                    nuevoProceso.ProductoId = dto.ProductoId;

                    await _repository.CreateAsync(nuevoProceso);
                }
            }
        }
    }
}
