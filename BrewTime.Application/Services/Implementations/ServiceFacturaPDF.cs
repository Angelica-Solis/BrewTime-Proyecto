using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceFacturaPDF : IServiceFacturaPDF
    {
        public byte[] GenerarFactura(PedidoDetalleDTO pedido, byte[] logo)
        {
            string M(decimal valor) => $"₡{valor:N2}";

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(28);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().BorderBottom(1).BorderColor("#D9D9D9").PaddingBottom(10).Row(row =>
                    {
                        row.ConstantItem(85).Height(50).AlignLeft().Element(c =>
                        {
                            if (logo != null && logo.Length > 0)
                                c.Image(logo).FitArea();
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text("FACTURA").FontSize(20).Bold().FontColor("#2C4725");
                            col.Item().AlignRight().Text($"Pedido #{pedido.PedidoId}").SemiBold();
                            col.Item().AlignRight().Text($"Fecha: {pedido.Fecha:dd/MM/yyyy HH:mm}");
                        });
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(12);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingRight(5).Background("#F8F1E5").Border(1).BorderColor("#E6D8C3").Padding(12).Column(x =>
                            {
                                x.Item().Text("Cliente").Bold().FontColor("#2C4725").FontSize(11);
                                x.Item().PaddingTop(4).Text(pedido.ClienteNombre ?? "");
                                x.Item().Text(pedido.ClienteCorreo ?? "");
                            });

                            row.RelativeItem().PaddingLeft(5).Background("#F8F1E5").Border(1).BorderColor("#E6D8C3").Padding(12).Column(x =>
                            {
                                x.Item().Text("Entrega y pago").Bold().FontColor("#2C4725").FontSize(11);
                                x.Item().PaddingTop(4).Text($"Método de entrega: {pedido.MetodoEntrega}");
                                if (!string.IsNullOrWhiteSpace(pedido.DireccionEntrega))
                                    x.Item().Text($"Dirección: {pedido.DireccionEntrega}");
                                x.Item().Text($"Método de pago: {pedido.MetodoPago}");
                            });
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.6f);
                                columns.RelativeColumn(1.8f);
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
                                table.Cell().Element(BodyCell).AlignRight().Text(M(detalle.Precio));
                                table.Cell().Element(BodyCell).AlignRight().Text(M(detalle.Subtotal));
                            }
                        });

                        col.Item().AlignRight().Width(290).Background("#F8F1E5").Border(1).BorderColor("#E6D8C3").Padding(12).Column(total =>
                        {
                            total.Spacing(4);

                            total.Item().Text("Resumen de cobro").Bold().FontColor("#2C4725").FontSize(11);

                            TotalRow(total, "Subtotal:", M(pedido.Subtotal));
                            TotalRow(total, "Impuesto:", M(pedido.Impuesto));

                            if (pedido.CostoEnvio > 0)
                            {
                                total.Item().PaddingTop(4).Text("Desglose de envío").Bold().FontColor("#2C4725");
                                TotalRow(total, "Costo base:", M(pedido.CostoBaseEnvio));
                                TotalRow(total, "Costo por distancia:", M(pedido.CostoPorDistancia));
                                TotalRow(total, "Total envío:", M(pedido.CostoEnvio), true);
                            }

                            total.Item().PaddingTop(6).BorderTop(1).BorderColor("#2C4725").PaddingTop(6).Row(row =>
                            {
                                row.RelativeItem().Text("TOTAL:").Bold().FontSize(13).FontColor("#2C4725");
                                row.ConstantItem(110).AlignRight().Text(M(pedido.Total)).Bold().FontSize(13).FontColor("#2C4725");
                            });

                            if (pedido.MontoPagado.HasValue)
                                TotalRow(total, "Monto pagado:", M(pedido.MontoPagado.Value));

                            if (pedido.Vuelto.HasValue && pedido.Vuelto.Value > 0)
                                TotalRow(total, "Vuelto:", M(pedido.Vuelto.Value));

                            if (!string.IsNullOrWhiteSpace(pedido.UltimosDigitosTarjeta))
                                total.Item().PaddingTop(2).AlignRight().Text($"Tarjeta: **** {pedido.UltimosDigitosTarjeta}");
                        });

                        col.Item().PaddingTop(10).AlignCenter().Text("Gracias por elegir BrewTime").Bold().FontColor("#2C4725");
                    });

                    page.Footer().PaddingTop(8).BorderTop(1).BorderColor("#D9D9D9").AlignCenter()
                        .DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium))
                        .Text(x =>
                        {
                            x.Span("BrewTime • Factura generada electrónicamente • Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return documento.GeneratePdf();
        }

        private static void TotalRow(ColumnDescriptor total, string etiqueta, string valor, bool bold = false)
        {
            total.Item().Row(row =>
            {
                if (bold)
                {
                    row.RelativeItem().Text(etiqueta).Bold();
                    row.ConstantItem(110).AlignRight().Text(valor).Bold();
                }
                else
                {
                    row.RelativeItem().Text(etiqueta);
                    row.ConstantItem(110).AlignRight().Text(valor);
                }
            });
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container.Background("#2C4725").PaddingVertical(8).PaddingHorizontal(6)
                .DefaultTextStyle(x => x.FontColor("#F8EAD2").Bold());
        }

        private static IContainer BodyCell(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(7).PaddingHorizontal(6);
        }
    }
}