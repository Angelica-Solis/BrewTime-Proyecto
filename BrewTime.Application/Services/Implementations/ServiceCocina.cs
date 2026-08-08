using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceCocina : IServiceCocina
    {
        private readonly IRepositoryCocina _repository;

        public ServiceCocina(IRepositoryCocina repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PedidoCocinaDTO>> ObtenerPedidosCocinaAsync()
        {
            // primero obtener los pedidos en estado aceptado
            var pedidosAceptados = await _repository.ObtenerPedidosAceptadosAsync();

            // cambiarles el estado a esos pedidos a en preparacion
            foreach (var pedido in pedidosAceptados)
            {
                await _repository.CambiarAEnPreparacionAsync(pedido.PedidoId);
            }

            // se obtienen los pedidos que estan en preparacion 
            var pedidos = await _repository.ObtenerPedidosEnPreparacionAsync();

            return pedidos.Select(MapearPedido).ToList();
        }

        public async Task<bool> MarcarPedidoEnCaminoAsync(int pedidoId)
        {
            return await _repository.MarcarPedidoEnCaminoAsync(pedidoId);
        }

        private PedidoCocinaDTO MapearPedido(Pedido pedido)
        {
            var dto = new PedidoCocinaDTO
            {
                PedidoId = pedido.PedidoId,

                NombreCliente =
                    $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellidos}",

                FechaCreacion = pedido.FechaCreacion,

                Estado = pedido.Estado.Nombre
            };

            foreach (var detalle in pedido.PedidoDetalle)
            {
                // Producto individual
                if (detalle.Producto != null)
                {
                    var item = CrearItemProducto(
                        detalle,
                        detalle.Producto,
                        false,
                        null
                    );

                    AgregarPorCategoria(dto, item);
                }

                // Combo
                if (detalle.Combo != null)
                {
                    foreach (var comboProducto in detalle.Combo.ComboProducto)
                    {
                        var producto = comboProducto.Producto;

                        if (producto == null)
                            continue;

                        var cantidadTotal =
                            detalle.Cantidad * comboProducto.Cantidad;

                        var item = CrearItemProducto(
                            detalle,
                            producto,
                            true,
                            detalle.Combo.Nombre,
                            cantidadTotal
                        );

                        AgregarPorCategoria(dto, item);
                    }
                }
            }

            return dto;
        }

        private ItemCocinaDTO CrearItemProducto(PedidoDetalle detalle, Producto producto, bool esCombo, string? nombreCombo, int? cantidadCombo = null)
        {
            var item = new ItemCocinaDTO
            {
                DetalleId = detalle.DetalleId,

                ProductoId = producto.ProductoId,

                Nombre = producto.Nombre,

                Cantidad = cantidadCombo ?? detalle.Cantidad,

                EsCombo = esCombo,

                NombreCombo = nombreCombo,

                Categoria = producto.Categoria?.Nombre ?? string.Empty
            };

            // Obtener las estaciones configuradas para el producto
            foreach (var proceso in producto.ProcesoPreparacion
                .OrderBy(p => p.Orden))
            {
                var cola = detalle.ColaEstacion
                    .FirstOrDefault(c =>
                        c.EstacionId == proceso.EstacionId);

                item.Estaciones.Add(new EstacionCocinaDTO
                {
                    EstacionId = proceso.EstacionId,

                    Nombre = proceso.Estacion.Nombre,

                    Orden = proceso.Orden,

                    TiempoEstimadoMin = proceso.TiempoEstimadoMin,

                    Estado = cola?.Estado ?? "Pendiente",

                    FechaInicio = cola?.FechaInicio,

                    FechaFin = cola?.FechaFin
                });
            }

            return item;
        }

        private void AgregarPorCategoria(PedidoCocinaDTO pedido, ItemCocinaDTO item)
        {

            var categoria = item.Categoria
                .Trim()
                .ToLowerInvariant();

            if (categoria == "repostería")
            {
                pedido.Comida.Add(item);
            }
            else if (categoria == "cafés" ||
                     categoria == "café" ||
                     categoria == "cafe")
            {
                pedido.Cafe.Add(item);
            }
            else if (categoria == "bubble tea" ||
                     categoria == "bubbletea")
            {
                pedido.BubbleTea.Add(item);
            }
        
        }
    }
}
