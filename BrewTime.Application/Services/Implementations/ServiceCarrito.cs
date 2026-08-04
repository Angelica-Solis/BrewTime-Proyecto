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
    public class ServiceCarrito : IServiceCarrito
    {
        private readonly IRepositoryCarrito _repository;
        private readonly IMapper _mapper;

        public ServiceCarrito(
            IRepositoryCarrito repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AgregarProductoAsync(int usuarioId, int productoId, int cantidad = 1)
        {
            var item = await _repository.GetProductoAsync(usuarioId, productoId);

            if (item != null)
            {
                item.Cantidad += cantidad;

                await _repository.UpdateAsync(item);
            }
            else
            {
                await _repository.AddAsync(new Carrito
                {
                    UsuarioId = usuarioId,
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    FechaAgregado = DateTime.Now
                });
            }

            await _repository.SaveChangesAsync();
        }

        public async Task AgregarComboAsync(int usuarioId, int comboId, int cantidad = 1)
        {
            var item = await _repository.GetComboAsync(usuarioId, comboId);

            if (item != null)
            {
                item.Cantidad += cantidad;

                await _repository.UpdateAsync(item);
            }
            else
            {
                await _repository.AddAsync(new Carrito
                {
                    UsuarioId = usuarioId,
                    ComboId = comboId,
                    Cantidad = cantidad,
                    FechaAgregado = DateTime.Now
                });
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<CarritoDTO> ObtenerCarritoAsync(int usuarioId)
        {
            var lista = await _repository.GetByUsuarioAsync(usuarioId);

            var dto = new CarritoDTO();

            dto.Items = _mapper.Map<List<CarritoItemDTO>>(lista);

            return dto;
        }

        public async Task ActualizarCantidadAsync(int carritoId, int cantidad)
        {
            var item = await _repository.FindByIdAsync(carritoId);

            if (item == null)
                return;

            if (cantidad <= 0)
            {
                await _repository.DeleteAsync(carritoId);
            }
            else
            {
                item.Cantidad = cantidad;
                await _repository.UpdateAsync(item);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task EliminarAsync(int carritoId)
        {
            await _repository.DeleteAsync(carritoId);

            await _repository.SaveChangesAsync();
        }

        public async Task VaciarAsync(int usuarioId)
        {
            await _repository.DeleteAllAsync(usuarioId);

            await _repository.SaveChangesAsync();
        }
    }
}
