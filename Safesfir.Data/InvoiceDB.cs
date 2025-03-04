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
                        // Add an image to the header
                        row.ConstantColumn(100).Column(col =>
                        {
                            col.Item().Image("Images/CentexProduce.png"); // Adjust the height of the image
                        });
                        row.ConstantColumn(300).PaddingLeft(10).AlignMiddle().Column(col =>
                        {
                            col.Item().Text(invoice.CompanyName).FontSize(15).Bold();
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Invoice").FontSize(16).Bold().AlignRight();
                        });
                    });

                    page.Content().Column(col =>
                    {
                        // Shipping address white space issue - adjusted padding and fixed the layout
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(2).Text("Bill To").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(2).Text("Ship To").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(2).Text($"Invoice #: {invoice.InvoiceNumber}").Bold().FontSize(10);
                            });

                            table.Cell().BorderLeft(1).BorderRight(1).Padding(2).Text(invoice.CustomerName).FontSize(10);
                            table.Cell().BorderLeft(1).Padding(2).Text(invoice.CustomerShippingAddressLine1).FontSize(10);
                            table.Cell().Border(1).Padding(2).Text($"Date: {invoice.InvoiceDate:MM/dd/yyyy}").FontSize(10);

                            table.Cell().BorderLeft(1).Padding(1).Padding(2).Text(invoice.CustomerBillingAddressLine1).FontSize(10);
                            table.Cell().BorderLeft(1).BorderRight(1).Padding(2).Text(invoice.CustomerShippingAddressLine2).FontSize(10);
                            table.Cell().Border(1).Padding(2).Text(invoice.DueDate != null ? $"Due Date: {invoice.DueDate:MM/dd/yyyy}" : "").FontSize(10);

                            table.Cell().BorderLeft(1).Padding(2).Text(invoice.CustomerBillingAddressLine2).FontSize(10);
                            table.Cell().BorderLeft(1).BorderRight(1).Padding(2).Text($"{invoice.CustomerShippingCity}, {invoice.CustomerShippingCountry}").FontSize(10);
                            table.Cell().Border(1).Padding(2).Text($"Terms: {invoice.Terms}").FontSize(10);

                            table.Cell().BorderLeft(1).BorderBottom(1).Padding(2).Text($"{invoice.CustomerBillingCity}, {invoice.CustomerBillingCountry}").FontSize(10);
                            table.Cell().BorderLeft(1).BorderBottom(1).BorderRight(1).Text("");
                            table.Cell().Border(1).Padding(2).Text(invoice.ShipDate != null ? $"Ship: {invoice.ShipDate:MM/dd/yyyy}" : "").FontSize(10);

                            table.Cell().Text("");
                            table.Cell().Text("");
                            table.Cell().Text("");
                        });
                        col.Item().Padding(1).AlignLeft().Text($"VIA: {invoice.Via}").FontSize(10);

                        // Adjusted Quantity Column and Description Column
                        col.Item().PaddingTop(5).PaddingBottom(0).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);  // Smaller Quantity column
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(60);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Quantity").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Item Code").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Description").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Price Each").Bold().FontSize(10);
                                header.Cell().Border(1).Padding(1).AlignCenter().Text("Amount").Bold().FontSize(10);
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Border(1).Padding(2).AlignRight().Text(item.Quantity.ToString()).FontSize(10);
                                table.Cell().Border(1).Padding(2).AlignLeft().Text(item.Name).FontSize(10);
                                table.Cell().Border(1).Padding(2).AlignLeft().Text(item.Description).FontSize(10);
                                table.Cell().Border(1).Padding(2).AlignRight().Text($"${item.Rate:F2}").FontSize(10);
                                table.Cell().Border(1).Padding(2).AlignRight().Text($"${(item.Quantity * item.Rate):F2}").FontSize(10);
                            }
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Padding(1).Text(invoice.Memo).FontSize(6);
                            table.Cell().Padding(1).AlignRight().Text($"Total: ${invoice.TotalAmount:F2}").FontSize(10).Bold();

                            table.Cell().Text("");
                        });
                    });
                    // Footer with text and image
                    page.Footer().Row(row =>
                    {
                        row.RelativeColumn();
                        row.ConstantColumn(70).AlignRight().AlignMiddle().PaddingRight(2).Column(col =>
                        {
                            col.Item().AlignRight().Text("Powered by").FontSize(8).Bold(); // Text
                        });
                        // Image in footer
                        row.ConstantColumn(80).Column(col =>
                        {
                            col.Item().Image("Images/ProduceX.png"); // Adjust the height of the image
                        });
                        row.RelativeItem();
                    });
                });
            }).GeneratePdf();

            //    Document.Create(container =>
            //{
            //    container.Page(page =>
            //    {
            //        page.Size(PageSizes.A4);
            //        page.Margin(20);

            //        page.Header().Row(row =>
            //        {
            //            row.RelativeItem().Column(col =>
            //            {
            //                col.Item().Text(invoice.CompanyName).FontSize(20).Bold();
            //                col.Item().Text(invoice.CompanyAddressLine1);
            //                col.Item().Text($"{invoice.CompanyCountry}");
            //            });
            //        });

            //        page.Content().Column(col =>
            //        {
            //            //col.Item().LineHorizontal(0.5f);

            //            col.Item().Table(table =>
            //            {
            //                table.ColumnsDefinition(columns =>
            //                {
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                });

            //                table.Header(header =>
            //                {
            //                    header.Cell().Border(1).Padding(1).Text("Bill To").Bold();
            //                    header.Cell().Border(1).Padding(1).Text("Ship To").Bold();
            //                });

            //                table.Cell().BorderLeft(1).BorderRight(1).Padding(3).Text(invoice.CustomerName);
            //                table.Cell().BorderLeft(1).BorderRight(1).Text("");
            //                table.Cell().Border(1).Padding(1).Text($"Invoice #: {invoice.InvoiceNumber}").Bold();

            //                table.Cell().BorderLeft(1).Padding(1).Padding(3).Text(invoice.CustomerBillingAddressLine1);
            //                table.Cell().BorderLeft(1).Padding(3).Text(invoice.CustomerShippingAddressLine1);
            //                table.Cell().Border(1).Padding(1).Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}");

            //                table.Cell().BorderLeft(1).Padding(1).Padding(3).Text(invoice.CustomerBillingAddressLine2);
            //                table.Cell().BorderLeft(1).BorderRight(1).Padding(3).Text(invoice.CustomerShippingAddressLine2);
            //                table.Cell().Border(1).Padding(1).Text(invoice.DueDate != null ? $"Due Date: {invoice.DueDate:yyyy-MM-dd}" : "");

            //                table.Cell().BorderLeft(1).BorderBottom(1).Padding(3).Text($"{invoice.CustomerBillingCity}, {invoice.CustomerBillingCountry}");
            //                table.Cell().BorderLeft(1).BorderBottom(1).BorderRight(1).Padding(3).Text($"{invoice.CustomerShippingCity}, {invoice.CustomerShippingCountry}");
            //                table.Cell().Border(1).Padding(1).Text($"Terms: {invoice.Terms}");

            //                //table.Cell().Border(1).Text($"Phone: {invoice.CustomerPhoneNumber}");
            //                table.Cell().Text("");
            //                table.Cell().Text("");
            //                table.Cell().Border(1).Padding(1).Text($"Ship: {invoice.ShipDate}");
            //            });

            //            //col.Item().LineHorizontal(0.5f);

            //            col.Item().PaddingTop(3).PaddingBottom(0).Table(table =>
            //            {
            //                table.ColumnsDefinition(columns =>
            //                {
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                });

            //                table.Header(header =>
            //                {
            //                    header.Cell().Border(1).Padding(1).AlignCenter().Text("Quantity").Bold();
            //                    header.Cell().Border(1).Padding(1).AlignCenter().Text("Item Code").Bold();
            //                    header.Cell().Border(1).Padding(1).AlignCenter().Text("Description").Bold();
            //                    header.Cell().Border(1).Padding(1).AlignCenter().Text("Price Each").Bold();
            //                    header.Cell().Border(1).Padding(1).AlignCenter().Text("Amount").Bold();
            //                });

            //                foreach (var item in invoice.Items)
            //                {
            //                    table.Cell().Border(1).Padding(1).AlignRight().Text(item.Quantity.ToString());
            //                    table.Cell().Border(1).Padding(1).AlignLeft().Text(item.Name);
            //                    table.Cell().Border(1).Padding(1).AlignLeft().Text(item.Description);
            //                    table.Cell().Border(1).Padding(1).AlignRight().Text($"${item.Rate:F2}");
            //                    table.Cell().Border(1).Padding(1).AlignRight().Text($"${(item.Quantity * item.Rate):F2}");
            //                }
            //            });

            //            col.Item().Table(table =>
            //            {
            //                table.ColumnsDefinition(columns =>
            //                {
            //                    columns.RelativeColumn();
            //                    columns.RelativeColumn();
            //                });

            //                table.Cell().Padding(1).Text(invoice.Memo);
            //                table.Cell().Padding(1).Text("");
            //                table.Cell().Padding(1).Text("");
            //                table.Cell().Padding(1).AlignRight().Text($"Total: ${invoice.TotalAmount:F2}").FontSize(14).Bold();
            //                table.Cell().Padding(1).AlignCenter().Text($"VIA: {invoice.Via}").Bold();

            //                table.Cell().Text("");
            //            });
            //        });

            //        //page.Footer().AlignCenter().Text("Thank you for your business!");
            //    });
            //}).GeneratePdf();

        }
    }
}
