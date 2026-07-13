using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
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

        public ServiceMenu(
            IRepositoryMenu repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ── Lectura ──────────────────────────────────────────

        public async Task<ICollection<MenuDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();

            return _mapper.Map<ICollection<MenuDTO>>(list);
        }

        public async Task<MenuDTO?> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);

            return entity == null
                ? null
                : _mapper.Map<MenuDTO>(entity);
        }

        public async Task<MenuFormDTO?> FindFormByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);

            return entity == null
                ? null
                : _mapper.Map<MenuFormDTO>(entity);
        }

        public async Task<MenuDTO?> FindDisponibleAsync(
            DateOnly fechaActual,
            TimeOnly horaActual)
        {
            var entity = await _repository.FindDisponibleAsync(
                fechaActual,
                horaActual);

            return entity == null
                ? null
                : _mapper.Map<MenuDTO>(entity);
        }

        public async Task CreateAsync(MenuFormDTO dto)
        {
            var entity = _mapper.Map<Menu>(dto);

            await _repository.CreateAsync(
                entity,
                dto.ProductosSeleccionados,
                dto.CombosSeleccionados,
                dto.DiasSeleccionados);
        }

        public async Task UpdateAsync(MenuFormDTO dto)
        {
            var entity = await _repository.FindByIdAsync(dto.MenuId);

            if (entity == null)
                throw new KeyNotFoundException(
                    "El menú seleccionado no existe.");

            /*
             * Mapea el DTO sobre la entidad existente.
             * Las relaciones se ignoran en el Profile y se
             * actualizan dentro del repositorio.
             */
            _mapper.Map(dto, entity);

            await _repository.UpdateAsync(
                entity,
                dto.ProductosSeleccionados,
                dto.CombosSeleccionados,
                dto.DiasSeleccionados);
        }

        public async Task ToggleActivoAsync(int id)
        {
            await _repository.ToggleActivoAsync(id);
        }

        public async Task<bool> ExisteNombreAsync(string nombre,int? menuIdExcluir = null)
        {
            return await _repository.ExisteNombreAsync(
                nombre,
                menuIdExcluir);
        }
    }
}
