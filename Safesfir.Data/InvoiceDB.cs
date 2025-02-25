using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safesfir.Data
{
    public class InvoiceDB
    {
        public byte[] GenerateInvoicePdf(InvoiceModel invoice)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
                                                                    {
                                                                        container.Page(page =>
                                                                        {
                                                                            page.Size(PageSizes.A4);
                                                                            page.Margin(20);
                                                                            page.Header().Row(row =>
                                                                            {
                                                                                row.RelativeItem().Column(col =>
                                                                                {
                                                                                    col.Item().Text("INVOICE").FontSize(20).Bold();
                                                                                    //col.Item().Text("QuickBooks-Style Format").FontSize(10);
                                                                                });

                                                                                //row.ConstantItem(100).Image(Placeholders.Image(100, 50)); // Placeholder for logo
                                                                            });

                                                                            page.Content().Column(col =>
                                                                            {
                                                                                col.Item().Row(row =>
                                                                                {
                                                                                    row.RelativeItem().Column(innerCol =>
                                                                                    {
                                                                                        innerCol.Item().Text("Bill To:").Bold();
                                                                                        innerCol.Item().Text(invoice.CustomerName);
                                                                                        innerCol.Item().Text(invoice.CustomerAddress);
                                                                                    });

                                                                                    row.RelativeItem().Column(innerCol =>
                                                                                    {
                                                                                        innerCol.Item().Text($"Invoice #: {invoice.InvoiceNumber}").Bold();
                                                                                        innerCol.Item().Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}");
                                                                                        if (invoice.DueDate != null)
                                                                                            innerCol.Item().Text($"Due Date: {invoice.DueDate:yyyy-MM-dd}");
                                                                                    });
                                                                                });

                                                                                col.Item().LineHorizontal(0.5f);

                                                                                col.Item().Table(table =>
                                                                                {
                                                                                    table.ColumnsDefinition(columns =>
                                                                                    {
                                                                                        columns.RelativeColumn();
                                                                                        columns.RelativeColumn();
                                                                                        columns.RelativeColumn();
                                                                                        columns.RelativeColumn();
                                                                                    });

                                                                                    table.Header(header =>
                                                                                    {
                                                                                        header.Cell().Text("Item").Bold();
                                                                                        header.Cell().Text("Quantity").Bold();
                                                                                        header.Cell().Text("Rate").Bold();
                                                                                        header.Cell().Text("Amount").Bold();
                                                                                    });

                                                                                    foreach (var item in invoice.Items)
                                                                                    {
                                                                                        table.Cell().Text(item.Description);
                                                                                        table.Cell().Text(item.Quantity.ToString());
                                                                                        table.Cell().Text($"${item.Rate:F2}");
                                                                                        table.Cell().Text($"${(item.Quantity * item.Rate):F2}");
                                                                                    }
                                                                                });

                                                                                col.Item().AlignRight().Text($"Total: ${invoice.TotalAmount:F2}").FontSize(14).Bold();
                                                                            });

                                                                            page.Footer().AlignCenter().Text("Thank you for your business!");
                                                                        });
                                                                    }).GeneratePdf();

        }
    }
}
