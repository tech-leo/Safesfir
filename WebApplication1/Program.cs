using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Safesfir.Data;

namespace QBWCService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddServiceModelServices();
            builder.Services.AddServiceModelMetadata();
            builder.Services.AddSingleton<QBWebService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGet("/", () => "API is running!");
///generate-invoice?userid=67aa5a090c1e96f3d78c88f6&id=45A0-1071528830

//dotnet publish Safesfir.WebService.csproj

//cd bin/Release/net8.0/publish/

//sudo lsof -i :5000
//sudo kill -9 78055

//nohup dotnet Safesfir.WebService.dll --urls "http://0.0.0.0:5000"

            ///generate-invoice?userid=67aa5a090c1e96f3d78c88f6&id=E-1740272847
            app.MapGet("/clear-invoice", async () =>
            {
                var mongodbService = new MongoDBService();
                var users = await mongodbService.GetUsersAsync();
                users= users.Where(o=> o.QuickBooks.Connected==true && o.QuickBooks.Type== QuickBooksType.QBWD).ToList();
                foreach (var user in users) { 
                    if(user.DriverInvoice!=null) 
                    {
                        var ids= user.DriverInvoice.Select(o=> o.Id).ToList();
                        user.InvoicePayments = user.InvoicePayments.Where(o => !ids.Contains(o.InvoiceId)).ToList();
                    }
                    user.Invoices = new List<Invoice?>();
                    user.DriverInvoice = new List<DriverInvoice>();
                    user.QuickBooksCompany = new Company();
                    user.QuickBooks.Connected = false;
                    user.QuickBooks.QbwdCompany = string.Empty;
                    await mongodbService.UpdateUserAsync(user.Id, user);

                }
                return Results.Text($"Cleared Invoices of {users?.Count} users");
            });
            app.MapGet("/generate-invoice", async (string userid, string id) =>
            
            {

                var mongodbService = new MongoDBService();
                var user = await mongodbService.GetUserByIdAsync(userid);
                var invoice= user.Invoices.FirstOrDefault(o => o.TxnID == id);
                var invoiceDb= new InvoiceDB();
                var model = new InvoiceModel {
                    CustomerName = invoice.CustomerRef.FullName,
                    CompanyName = user.QuickBooksCompany?.CompanyName,
                    CompanyAddressLine1 = user.QuickBooksCompany.Address.Addr1,
                    CompanyCountry = user.QuickBooksCompany.Address.Country,
                    CustomerBillingAddressLine1 = invoice.BillAddress?.Addr1,
                    CustomerBillingAddressLine2 = invoice.BillAddress?.Addr2,
                    CustomerBillingCity = invoice.BillAddress?.City,
                    CustomerBillingCountry = invoice.BillAddress?.Country,
                    CustomerShippingCity = invoice.ShipAddress?.City,
                    CustomerShippingAddressLine1 = invoice.ShipAddress?.Addr1,
                    CustomerShippingAddressLine2 = invoice.ShipAddress?.Addr2,
                    CustomerShippingCountry = invoice.ShipAddress?.Country,
                    Memo = builder.Configuration["Memo"],
                    Terms = invoice.TermsRef?.FullName,
                    Via = invoice.DataExtRet.FirstOrDefault(p => p.DataExtName.ToLower() == "via")?.DataExtValue,
                    ShipDate = invoice.ShipDate,
                    
                    DueDate = !string.IsNullOrEmpty(invoice.DueDate)?DateTime.Parse(invoice.DueDate):null,
                    InvoiceDate = invoice.TxnDate ?? DateTime.UtcNow, 
                    InvoiceNumber = invoice.RefNumber
                };
                model.Items=new List<InvoiceItem>();
                foreach (var item in invoice.InvoiceLineRet)
                {
                    model.Items.Add(new InvoiceItem
                    {
                        Name=item.ItemRef?.FullName,
                        Description = item.Desc,
                        Quantity = item.Quantity ?? 1,
                        Rate = item.Rate ?? 0,
                    });
                }
                var pdfBytes= invoiceDb.GenerateInvoicePdf(model);
                return Results.File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
            });
            app.UseHttpsRedirection();


            var baseAddress = builder.Configuration["WCFSettings:BaseAddress"];
            app.UseServiceModel(builder =>
            {
                builder.ConfigureServiceHostBase<QBWebService>(host =>
                {
                    host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                });
                
                builder.AddService<QBWebService>()
                       .AddServiceEndpoint<QBWebService, IQBWebService>(new BasicHttpBinding()
                       {
                           MaxReceivedMessageSize = 1004857600,  // 100 MB
                           MaxBufferSize = 1004857600,
                           //MaxBufferPoolSize = 5242880,
                           ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                           {
                               MaxStringContentLength = 1004857600,
                               MaxArrayLength = 1004857600
                           }
                       }, "/soap");
                var serviceMetadata = app.Services.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadata.HttpsGetEnabled = false;
                serviceMetadata.HttpGetEnabled = true;  // Disable HTTP WSDL

                serviceMetadata.HttpsGetUrl = new Uri(baseAddress + "/soap?wsdl");
            });

            app.Run();
        }
    }
}
