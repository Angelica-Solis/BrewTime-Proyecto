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

            const string VerdeCafe = "#1F3D2B";
            const string VerdeSuave = "#3F6B4F";
            const string Crema = "#FBF7EF";
            const string CremaOscura = "#F0E7D6";
            const string Cafe = "#6B4F3A";
            const string CafeClaro = "#9A8770";
            const string Texto = "#33302B";
            const string Linea = "#E3D8C4";
            const string Dorado = "#B7924B";
            const string DoradoSuave = "#D8C7A0";
            const string Blanco = "#FFFFFF";
            const string ZebraFila = "#F8F3E8";

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(34);
                    page.MarginVertical(30);

                    page.DefaultTextStyle(x =>
                      x.FontFamily("Arial")
                       .FontSize(9.5f)
                       .FontColor(Texto));

                    // encabezado
                    page.Header()
                        .Column(headerCol =>
                        {
                            headerCol.Item()
                                .Height(3)
                                .Background(Dorado);

                            headerCol.Item()
                                .Background(VerdeCafe)
                                .Padding(18)
                                .Row(row =>
                                {
                                    // Logo
                                    row.ConstantItem(65)
                                        .Height(50)
                                        .AlignLeft()
                                        .AlignMiddle()
                                        .Element(c =>
                                        {
                                            if (logo != null && logo.Length > 0)
                                                c.Image(logo).FitArea();
                                        });

                                    row.ConstantItem(14);

                                    row.ConstantItem(1)
                                        .Height(46)
                                        .Background(DoradoSuave);

                                    row.ConstantItem(14);

                                    // Información de BrewTime
                                    row.RelativeItem()
                                        .AlignLeft()
                                        .AlignMiddle()
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .Text("BrewTime")
                                                .FontSize(15)
                                                .Bold()
                                                .FontColor(Crema);

                                            col.Item()
                                                .PaddingTop(3)
                                                .Text("Café • Sabor • Experiencia")
                                                .FontSize(8)
                                                .Italic()
                                                .FontColor(DoradoSuave);
                                        });

                                    // Factura
                                    row.RelativeItem()
                                        .AlignRight()
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .AlignRight()
                                                .Text("Factura")
                                                .FontSize(15)
                                                .Bold()
                                                .FontColor(Dorado);

                                            col.Item()
                                                .PaddingTop(3)
                                                .AlignRight()
                                                .Text($"Pedido #{pedido.PedidoId}")
                                                .FontSize(10)
                                                .SemiBold()
                                                .FontColor(Crema);

                                            col.Item()
                                                .PaddingTop(2)
                                                .AlignRight()
                                                .Text($"{pedido.Fecha:dd/MM/yyyy HH:mm}")
                                                .FontSize(8.5f)
                                                .FontColor(DoradoSuave);
                                        });
                                });
                        });


                    // contenido
                    page.Content()
                        .PaddingTop(22)
                        .Column(col =>
                        {
                            col.Spacing(16);

                            // información del cliente / entrega
                            col.Item().Row(row =>
                            {
                                row.RelativeItem()
                                    .PaddingRight(7)
                                    .Background(Crema)
                                    .BorderLeft(3)
                                    .BorderColor(Dorado)
                                    .Padding(14)
                                    .Column(x =>
                                    {
                                        x.Item()
                                            .Text("Cliente")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(VerdeCafe);

                                        x.Item()
                                            .PaddingTop(8)
                                            .Text(pedido.ClienteNombre ?? "")
                                            .FontSize(10.5f)
                                            .Bold()
                                            .FontColor(Texto);

                                        x.Item()
                                            .PaddingTop(3)
                                            .Text(pedido.ClienteCorreo ?? "")
                                            .FontSize(8.5f)
                                            .Italic()
                                            .FontColor(CafeClaro);
                                    });

                                row.RelativeItem()
                                    .PaddingLeft(7)
                                    .Background(Crema)
                                    .BorderLeft(3)
                                    .BorderColor(Dorado)
                                    .Padding(14)
                                    .Column(x =>
                                    {
                                        x.Item()
                                            .Text("Entrega Y Pago")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(VerdeCafe);

                                        x.Item()
                                            .PaddingTop(8)
                                            .Text($"Entrega:  {pedido.MetodoEntrega}")
                                            .FontSize(8.8f);

                                        if (!string.IsNullOrWhiteSpace(pedido.DireccionEntrega))
                                        {
                                            x.Item()
                                                .PaddingTop(3)
                                                .Text($"Dirección:  {pedido.DireccionEntrega}")
                                                .FontSize(8.5f)
                                                .FontColor(CafeClaro);
                                        }

                                        x.Item()
                                            .PaddingTop(3)
                                            .Text($"Pago:  {pedido.MetodoPago}")
                                            .FontSize(8.8f);
                                    });
                            });

                            // detalle de productos
                            col.Item()
                                .Column(section =>
                                {
                                    section.Item()
                                        .PaddingBottom(7)
                                        .Row(r =>
                                        {
                                            r.AutoItem()
                                                .Text("Detalle Del Pedido")
                                                .FontSize(10.5f)
                                                .Bold()
                                                .FontColor(VerdeCafe);

                                            r.RelativeItem()
                                                .PaddingLeft(10)
                                                .AlignMiddle()
                                                .Height(1)
                                                .Background(Linea);
                                        });

                                    section.Item().Table(table =>
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

                                        for (int i = 0; i < pedido.Detalles.Count; i++)
                                        {
                                            var detalle = pedido.Detalles[i];
                                            var bg = i % 2 == 0 ? Blanco : ZebraFila;

                                            table.Cell().Element(c => BodyCell(c, bg)).Text(detalle.Producto);
                                            table.Cell().Element(c => BodyCell(c, bg)).AlignCenter().Text(detalle.Cantidad.ToString());
                                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(M(detalle.Precio));
                                            table.Cell().Element(c => BodyCell(c, bg)).AlignRight().Text(M(detalle.Subtotal)).SemiBold().FontColor(VerdeCafe);
                                        }
                                    });
                                });

                            // resumen
                            col.Item()
                                .AlignRight()
                                .Width(290)
                                .Background(Crema)
                                .BorderLeft(3)
                                .BorderColor(Dorado)
                                .Padding(16)
                                .Column(total =>
                                {
                                    total.Spacing(5);

                                    total.Item()
                                        .Text("Resumen de Cobro")
                                        .Bold()
                                        .FontSize(9.5f)
                                        .FontColor(VerdeCafe);

                                    total.Item().PaddingTop(3);

                                    TotalRow(total, "Subtotal:", M(pedido.Subtotal), Texto);
                                    TotalRow(total, "Impuesto:", M(pedido.Impuesto), Texto);

                                    if (pedido.CostoEnvio > 0)
                                    {
                                        total.Item()
                                            .PaddingTop(6)
                                            .Text("Desglose de Envío")
                                            .Bold()
                                            .FontSize(8)
                                            .FontColor(Cafe);

                                        TotalRow(total, "Costo base:", M(pedido.CostoBaseEnvio), CafeClaro);
                                        TotalRow(total, "Costo por distancia:", M(pedido.CostoPorDistancia), CafeClaro);
                                        TotalRow(total, "Total envío:", M(pedido.CostoEnvio), Cafe, true);
                                    }

                                    total.Item()
                                        .PaddingTop(9)
                                        .BorderTop(1.5f)
                                        .BorderColor(Dorado)
                                        .PaddingTop(9)
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text("Total")
                                                .Bold()
                                                .FontSize(12.5f)
                                                .FontColor(VerdeCafe);

                                            row.ConstantItem(115)
                                                .AlignRight()
                                                .Text(M(pedido.Total))
                                                .Bold()
                                                .FontSize(14)
                                                .FontColor(VerdeCafe);
                                        });

                                    if (pedido.MontoPagado.HasValue)
                                    {
                                        total.Item().PaddingTop(4);
                                        TotalRow(total, "Monto pagado:", M(pedido.MontoPagado.Value), Texto);
                                    }

                                    if (pedido.Vuelto.HasValue && pedido.Vuelto.Value > 0)
                                    {
                                        TotalRow(total, "Vuelto:", M(pedido.Vuelto.Value), Texto);
                                    }

                                    if (!string.IsNullOrWhiteSpace(pedido.UltimosDigitosTarjeta))
                                    {
                                        total.Item()
                                            .PaddingTop(5)
                                            .AlignRight()
                                            .Text($"Tarjeta terminada en {pedido.UltimosDigitosTarjeta}")
                                            .FontSize(8)
                                            .Italic()
                                            .FontColor(CafeClaro);
                                    }
                                });

                            // mensaje final
                            col.Item()
                                .PaddingTop(8)
                                .AlignCenter()
                                .Column(thanks =>
                                {
                                    thanks.Item()
                                        .AlignCenter()
                                        .Width(60)
                                        .Height(1)
                                        .Background(DoradoSuave);

                                    thanks.Item()
                                        .PaddingTop(10)
                                        .Text("")
                                        .FontSize(15)
                                        .FontColor(Dorado);

                                    thanks.Item()
                                        .PaddingTop(4)
                                        .Text("¡Gracias por elegir BrewTime!")
                                        .Bold()
                                        .FontSize(11.5f)
                                        .FontColor(VerdeCafe);

                                    thanks.Item()
                                        .PaddingTop(2)
                                        .Text("Esperamos volver a compartir un buen café contigo.")
                                        .FontSize(8)
                                        .Italic()
                                        .FontColor(CafeClaro);
                                });
                        });

                    // pie de página
                    page.Footer()
                        .Column(footerCol =>
                        {
                            footerCol.Item()
                                .Height(1)
                                .Background(Linea);

                            footerCol.Item()
                                .PaddingTop(8)
                                .AlignCenter()
                                .DefaultTextStyle(x =>
                                    x.FontSize(7.5f)
                                     .FontColor(CafeClaro))
                                .Text(x =>
                                {
                                    x.Span("BrewTime  ·  Factura generada electrónicamente  ·  Página ");
                                    x.CurrentPageNumber();
                                });
                        });
                });
            });

            return documento.GeneratePdf();
        }

        private static void TotalRow(
            ColumnDescriptor total,
            string etiqueta,
            string valor,
            string colorTexto,
            bool bold = false)
        {
            total.Item().Row(row =>
            {
                if (bold)
                {
                    row.RelativeItem().Text(etiqueta).Bold().FontColor(colorTexto);
                    row.ConstantItem(110).AlignRight().Text(valor).Bold().FontColor(colorTexto);
                }
                else
                {
                    row.RelativeItem().Text(etiqueta).FontColor(colorTexto);
                    row.ConstantItem(110).AlignRight().Text(valor).FontColor(colorTexto);
                }
            });
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container
                .Background("#1F3D2B")
                .PaddingVertical(9)
                .PaddingHorizontal(8)
                .DefaultTextStyle(x =>
                    x.FontColor("#FBF7EF")
                     .Bold()
                     .FontSize(8.5f));
        }

        private static IContainer BodyCell(IContainer container, string backgroundColor)
        {
            return container
                .Background(backgroundColor)
                .BorderBottom(0.6f)
                .BorderColor("#E3D8C4")
                .PaddingVertical(8)
                .PaddingHorizontal(8)
                .DefaultTextStyle(x =>
                    x.FontColor("#33302B")
                     .FontSize(8.8f));
        }
    }
}