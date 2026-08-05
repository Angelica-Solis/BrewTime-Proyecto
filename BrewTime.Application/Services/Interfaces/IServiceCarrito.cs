using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCarrito
    {
        Task AgregarProductoAsync(int usuarioId, int productoId, int cantidad = 1);

        Task AgregarComboAsync(int usuarioId, int comboId, int cantidad = 1);

        Task<CarritoDTO> ObtenerCarritoAsync(int usuarioId);

        Task ActualizarCantidadAsync(int carritoId, int cantidad);

        Task EliminarAsync(int carritoId);

        Task VaciarAsync(int usuarioId);
        Task<int> CantidadItemsAsync(int usuarioId);

    }
}
