using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceFacturaPDF : IServiceFacturaPDF
    {
        public byte[] GenerarFactura(PedidoDetalleDTO pedido, byte[] logo)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Width(120).Image(logo);
                        col.Item().PaddingTop(10).AlignCenter()
                            .Text($"FACTURA - PEDIDO #{pedido.PedidoId}")
                            .FontSize(16).Bold().FontColor("#2C4725");
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(12);

                        col.Item().Background("#F8F1E5").Padding(12).Column(info =>
                        {
                            info.Item().Text("Información del pedido").Bold().FontSize(12);
                            info.Item().Text($"Fecha: {pedido.Fecha:dd/MM/yyyy HH:mm}");
                            info.Item().Text($"Cliente: {pedido.ClienteNombre}");
                            info.Item().Text($"Correo: {pedido.ClienteCorreo}");
                            info.Item().Text($"Método de entrega: {pedido.MetodoEntrega}");

                            if (!string.IsNullOrWhiteSpace(pedido.DireccionEntrega))
                                info.Item().Text($"Dirección: {pedido.DireccionEntrega}");

                            info.Item().Text($"Método de pago: {pedido.MetodoPago}");
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Producto");
                                header.Cell().Element(HeaderCell).AlignCenter().Text("Cant.");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Precio");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
                            });

                            foreach (var detalle in pedido.Detalles)
                            {
                                table.Cell().Element(BodyCell).Text(detalle.Producto);
                                table.Cell().Element(BodyCell).AlignCenter().Text(detalle.Cantidad.ToString());
                                table.Cell().Element(BodyCell).AlignRight().Text($"₡{detalle.Precio:N2}");
                                table.Cell().Element(BodyCell).AlignRight().Text($"₡{detalle.Subtotal:N2}");
                            }
                        });

                        col.Item().AlignRight().Width(250).Column(total =>
                        {
                            total.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Subtotal:");
                                row.ConstantItem(110).AlignRight().Text($"₡{pedido.Subtotal:N2}");
                            });

                            total.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Impuesto:");
                                row.ConstantItem(110).AlignRight().Text($"₡{pedido.Impuesto:N2}");
                            });

                            if (pedido.CostoEnvio > 0)
                            {
                                total.Item().PaddingTop(4).Text("Costo de envío").Bold();

                                total.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Costo base:");
                                    row.ConstantItem(110).AlignRight().Text($"₡{pedido.CostoBaseEnvio:N2}");
                                });

                                total.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Costo por distancia:");
                                    row.ConstantItem(110).AlignRight().Text($"₡{pedido.CostoPorDistancia:N2}");
                                });

                                total.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Total envío:").Bold();
                                    row.ConstantItem(110).AlignRight().Text($"₡{pedido.CostoEnvio:N2}").Bold();
                                });
                            }

                            total.Item().PaddingTop(5).BorderTop(1).BorderColor("#2C4725").Row(row =>
                            {
                                row.RelativeItem().Text("TOTAL:").Bold().FontSize(13);
                                row.ConstantItem(110).AlignRight().Text($"₡{pedido.Total:N2}").Bold().FontSize(13).FontColor("#2C4725");
                            });

                            if (pedido.MontoPagado.HasValue)
                            {
                                total.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Monto pagado:");
                                    row.ConstantItem(110).AlignRight().Text($"₡{pedido.MontoPagado.Value:N2}");
                                });
                            }

                            if (pedido.Vuelto.HasValue && pedido.Vuelto.Value > 0)
                            {
                                total.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Vuelto:");
                                    row.ConstantItem(100).AlignRight().Text($"₡{pedido.Vuelto.Value:N2}");
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(pedido.UltimosDigitosTarjeta))
                                total.Item().Text($"Tarjeta: **** {pedido.UltimosDigitosTarjeta}").AlignRight();
                        });

                        col.Item().PaddingTop(20).AlignCenter()
                            .Text("Gracias por elegir BrewTime")
                            .FontSize(12).Bold().FontColor("#2C4725");
                    });

                    page.Footer().AlignCenter()
                        .DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium)).Text(x =>
                        {
                            x.Span("BrewTime • Factura generada electrónicamente • Página ");
                            x.CurrentPageNumber();
                         });
                });
            });

            return documento.GeneratePdf();
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container.Background("#2C4725").Padding(7).DefaultTextStyle(x => x.FontColor("#F8EAD2").Bold());
        }

        private static IContainer BodyCell(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(7);
        }
    }
}
