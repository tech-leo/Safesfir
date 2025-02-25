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
                            col.Item().Text(invoice.CompanyName).FontSize(20).Bold();
                            col.Item().Text(invoice.CompanyAddressLine1);
                            col.Item().Text($"{invoice.CompanyCountry}");
                        });
                    });

                    page.Content().Column(col =>
                    {
                        //col.Item().LineHorizontal(0.5f);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(1).Text("Bill To").Bold();
                                header.Cell().Border(1).Padding(1).Text("Ship To").Bold();
                            });

                            table.Cell().BorderLeft(1).BorderRight(1).Padding(3).Text(invoice.CustomerName);
                            table.Cell().BorderLeft(1).BorderRight(1).Text("");
                            table.Cell().Border(1).Padding(1).Text($"Invoice #: {invoice.InvoiceNumber}").Bold();
                                        
                            table.Cell().BorderLeft(1).Padding(1).Padding(3).Text(invoice.CustomerBillingAddressLine1);
                            table.Cell().BorderLeft(1).Padding(3).Text(invoice.CustomerShippingAddressLine1);
                            table.Cell().Border(1).Padding(1).Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}");
                                        
                            table.Cell().BorderLeft(1).Padding(1).Padding(3).Text(invoice.CustomerBillingAddressLine2);
                            table.Cell().BorderLeft(1).BorderRight(1).Padding(3).Text(invoice.CustomerShippingAddressLine2);
                            table.Cell().Border(1).Padding(1).Text(invoice.DueDate != null ? $"Due Date: {invoice.DueDate:yyyy-MM-dd}" : "");
                                        
                            table.Cell().BorderLeft(1).BorderBottom(1).Padding(3).Text($"{invoice.CustomerBillingCity}, {invoice.CustomerBillingCountry}");
                            table.Cell().BorderLeft(1).BorderBottom(1).BorderRight(1).Padding(3).Text($"{invoice.CustomerShippingCity}, {invoice.CustomerShippingCountry}");
                            table.Cell().Border(1).Padding(1).Text($"Terms: {invoice.Terms}");
                                        
                            //table.Cell().Border(1).Text($"Phone: {invoice.CustomerPhoneNumber}");
                            table.Cell().Text("");
                            table.Cell().Text("");
                            table.Cell().Border(1).Padding(1).Text($"Ship: {invoice.ShipDate}");
                        });

                        //col.Item().LineHorizontal(0.5f);

                        col.Item().PaddingTop(3).PaddingBottom(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Quantity").Bold();
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Item Code").Bold();
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Description").Bold();
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Price Each").Bold();
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Amount").Bold();
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Border(1).Padding(1).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Border(1).Padding(1).AlignLeft().Text(item.Name);
                                table.Cell().Border(1).Padding(1).AlignLeft().Text(item.Description);
                                table.Cell().Border(1).Padding(1).AlignRight().Text($"${item.Rate:F2}");
                                table.Cell().Border(1).Padding(1).AlignRight().Text($"${(item.Quantity * item.Rate):F2}");
                            }
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Padding(1).Text(invoice.Memo);
                            table.Cell().Padding(1).Text("");
                            table.Cell().Padding(1).Text("");
                            table.Cell().Padding(1).AlignRight().Text($"Total: ${invoice.TotalAmount:F2}").FontSize(14).Bold();
                            table.Cell().Padding(1).AlignCenter().Text($"VIA: {invoice.Via}").Bold();

                            table.Cell().Text("");
                        });
                    });

                    //page.Footer().AlignCenter().Text("Thank you for your business!");
                });
            }).GeneratePdf();

        }
    }
}
