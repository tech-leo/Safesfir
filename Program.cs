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
            app.MapGet("/generate-invoice", async (string userid, string id) =>
            
            {

                var mongodbService = new MongoDBService();
                var user = await mongodbService.GetUserByIdAsync(userid);
                var invoice= user.Invoices.FirstOrDefault(o => o.TxnID == id);
                var invoiceDb= new InvoiceDB();
                var model = new InvoiceModel { CustomerName = invoice.CustomerRef.FullName, DueDate = !string.IsNullOrEmpty(invoice.DueDate)?DateTime.Parse(invoice.DueDate):null,
                    InvoiceDate = invoice.TxnDate ?? DateTime.UtcNow, InvoiceNumber = invoice.RefNumber
                };
                model.Items=new List<InvoiceItem>();
                foreach (var item in invoice.InvoiceLineRet)
                {
                    model.Items.Add(new InvoiceItem
                    {
                        Description = item.Desc,
                        Quantity = item.Quantity ?? 1,
                        Rate = item.Rate ?? 0,
                    });
                }
                var pdfBytes= invoiceDb.GenerateInvoicePdf(model);
                return Results.File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
            });
            app.UseHttpsRedirection();

            //app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var baseAddress = builder.Configuration["WCFSettings:BaseAddress"];
            app.UseServiceModel(builder =>
            {
                builder.ConfigureServiceHostBase<QBWebService>(host =>
                {
                    host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                });
                
                builder.AddService<QBWebService>()
                       .AddServiceEndpoint<QBWebService, IQBWebService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                       {
                           MaxReceivedMessageSize = 1004857600,  // 100 MB
                           MaxBufferSize = 1004857600,
                           //MaxBufferPoolSize = 5242880,
                           ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                           {
                               MaxStringContentLength = 1004857600,
                               MaxArrayLength = 1004857600
                           }
                       }, "/QBWebService.svc");
                var serviceMetadata = app.Services.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadata.HttpsGetEnabled = true;
                serviceMetadata.HttpGetEnabled = false;  // Disable HTTP WSDL

                serviceMetadata.HttpsGetUrl = new Uri(baseAddress + "/QBWebService.svc?wsdl");
            });

            app.Run();
        }
    }
}
