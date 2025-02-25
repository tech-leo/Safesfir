using CoreWCF;
using CoreWCF.IdentityModel.Protocols.WSTrust;
using MongoDB.Bson;
using Newtonsoft.Json;
using Safesfir.Data;
using Safesfir.QBD;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace QBWCService
{
    [ServiceBehavior(Namespace = "http://developer.intuit.com/")]
    public class QBWebService : IQBWebService
    {
        private readonly MongoDBService mongodbService;
        public QBWebService()
        {
            this.mongodbService = new MongoDBService();
        }
        private static Dictionary<string, User> sessionTickets = new Dictionary<string, User>();

        public string serverVersion()
        {
            return "1.0"; // Return the version of your SOAP service
        }
        public string clientVersion(string productVersion)
        {
            // QBWC expects a string response, returning "" means continue normally
            return "";
        }
        public async Task<List<string>> authenticate(string strUserName, string strPassword)
        {
            var user = await mongodbService.GetUserByQBDUserNameAndPawordAsync(strUserName, strPassword);
            if (user != null)
            {

                string ticket = Guid.NewGuid().ToString();
                sessionTickets[ticket] = user;
                return new List<string> { ticket, "" }; // No company file restriction

            }
            return new List<string> { "nvu", "" }; // Not valid user
        }

        public async Task<string> sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName, string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers)
        {
            if (!sessionTickets.ContainsKey(ticket)) return "";
            var user = sessionTickets[ticket];
            var xmlGenerator = new QBDXMLGenerator();
            List<(string, string)> signedPDFURL = new List<(string, string)>(); 
            if (user != null)
            {
                user.QuickBooks.Connected = true;
                user.QuickBooks.Type = QuickBooksType.QBWD;
                user.QuickBooks.QbwdLastSync = DateTime.UtcNow;
                var xmlDoc = XElement.Parse(strHCPResponse);
                try
                {
                    var responses = new ParseQBXML<Company>().GetQueryRets(xmlDoc, "CompanyRet").Select(d => ("", d)).ToList();
                    user.QuickBooksCompany = responses.FirstOrDefault().Item2;
                    user.QuickBooks.QbwdCompany = user.QuickBooksCompany.CompanyName;
                }
                catch
                {
                }
                await mongodbService.UpdateUserAsync(user.Id, user);
                var drivers = await mongodbService.GetUsersAsync();
                var payments = drivers.SelectMany(p => p.InvoicePayments).ToList();
                var driverInvoices = drivers.SelectMany(p => p.DriverInvoice).Select(p=> p.Id).ToList();
                foreach (var payment in payments)
                {
                    if(!string.IsNullOrEmpty( payment.SignedPdfUrl) && payment.Status!="Pending" && driverInvoices.Contains(payment.InvoiceId))
                    {
                        signedPDFURL.Add((payment.InvoiceId, payment.SignedPdfUrl));
                    }
                }
            }

            var nodes= new List<string>();
            if(signedPDFURL.Count > 0)
            {
                nodes.Add("DataExtModRq");
            }
            nodes.AddRange(["InvoiceQueryRq", "ReceivePaymentQueryRq"]);
            var res = xmlGenerator.GetMultipleXmlDocument(nodes, signedPDFURL).OuterXml;
            return res;
            
        }

        public async Task<int> receiveResponseXML(string ticket, string response, string hresult, string message)
        {
            if (!sessionTickets.ContainsKey(ticket)) return -1;
            if (response != null && !string.IsNullOrEmpty(response))
            {
                await ProcessInvoice(response, ticket);
                //await ProcessPayment(response, ticket);
            }
            else
            {

            }
            return 100; // 100 means all data processed
        }
        private async Task ProcessInvoice(string response, string ticket)
        {
            var user = sessionTickets[ticket];
            var xmlDoc = XElement.Parse(response);

            var responses = new ParseQBXML<Invoice>().GetQueryRets(xmlDoc, "InvoiceRet").Select(d => ("", d)).ToList();
            var responsePayments = new ParseQBXML<Invoice>().GetQueryRets(xmlDoc, "ReceivePaymentRet").Select(d => ("", d)).ToList();

            if (responses.Count > 0)
            {

                foreach (var invoice in responses.Where(o => o.Item2 != null && o.Item2?.DataExtRet?.Any(p => p.DataExtName?.ToUpper() == "VIA") == true))
                {
                    if (invoice.Item2 != null)
                    {
                        string name = invoice.Item2.DataExtRet?.FirstOrDefault(o => o.DataExtName?.ToUpper() == "VIA")?.DataExtValue;
                        var driver = await mongodbService.GetDriverByNameAsync(name);
                        var addableinvoices = new List<Invoice>();
                        var addableDriverInvoices = new List<DriverInvoice>();

                        if (driver != null)
                        {
                            driver.QuickBooksCompany = user.QuickBooksCompany;
                            driver.QuickBooks = driver.QuickBooks ?? new QuickBooksInfo();
                            driver.QuickBooks.Type = QuickBooksType.QBWD;
                            driver.QuickBooks.Connected = true;
                            driver.QuickBooks.QbwdLastSync = DateTime.UtcNow;
                            if (driver.Invoices == null)
                            {
                                driver.Invoices = new List<Invoice>();
                            }
                            else
                            {
                                driver.Invoices = driver.Invoices.GroupBy(o => o.TxnID).Select(o => o.FirstOrDefault(o => o?.DataExtRet?.Any(p => p.DataExtName?.ToUpper() == "VIA") == true)).ToList();
                            }
                            if (driver.DriverInvoice == null)
                            {
                                driver.DriverInvoice = new List<DriverInvoice>();
                            }
                            else
                            {
                                driver.DriverInvoice = driver.DriverInvoice.GroupBy(o => o.Id).Select(o => o.FirstOrDefault()).ToList();
                            }
                            if (driver.Invoices.Count > 0)
                            {
                                for (int i = 0; i < driver.Invoices.Count; i++)
                                {

                                    if (driver.Invoices[i].TxnID == invoice.Item2?.TxnID)
                                    {

                                        var id = driver.Invoices[i].Id;
                                        driver.Invoices[i] = AutoMapper.MapBaseToDerived(invoice.Item2, driver.Invoices[i]);

                                        driver.Invoices[i].Id = id;
                                    }
                                    else if (!addableinvoices.Any(p => p.TxnID == invoice.Item2?.TxnID) && !driver.Invoices.Any(p => p.TxnID == invoice.Item2?.TxnID))
                                    {

                                        driver.Invoices[i].Id = ObjectId.GenerateNewId().ToString();
                                        addableinvoices.Add(invoice.Item2);
                                    }

                                    // ✅ Now modification is allowed
                                }
                            }
                            else
                            {
                                invoice.Item2.Id = ObjectId.GenerateNewId().ToString();
                                addableinvoices.Add(invoice.Item2);
                            }


                            var cf = new List<CustomField>();
                            foreach (var field in invoice.Item2.DataExtRet)
                            {
                                cf.Add(new CustomField
                                {
                                    DefinitionId = field.OwnerID,
                                    Name = field.DataExtName,
                                    StringValue = field.DataExtValue,
                                    Type = field.DataExtValue
                                });
                            }
                            var status = "Unpaid";

                            if (invoice.Item2.BalanceRemaining == 0)
                            {
                                status = "Completed";
                            }
                            else if (invoice.Item2.BalanceRemaining < invoice.Item2.BalanceRemaining)
                            {
                                status = "Partial";
                            }
                            if (driver.DriverInvoice.Count > 0)
                            {
                                for (int i = 0; i < driver.DriverInvoice.Count; i++)
                                {
                                    driver.DriverInvoice[i].paymentDetails = driver.InvoicePayments.FirstOrDefault(o => o.InvoiceId == invoice.Item2?.TxnID);
                                    if (driver.DriverInvoice[i].paymentDetails != null)
                                    {
                                        status = driver.DriverInvoice[i].paymentDetails.Status;
                                    }

                                    if (driver.DriverInvoice[i].Id == invoice.Item2?.TxnID)
                                    {

                                        var id = driver.DriverInvoice[i].uniqueId;

                                        driver.DriverInvoice[i] = new DriverInvoice
                                        {
                                            Id = invoice.Item2.TxnID,
                                            DocNumber = invoice.Item2.RefNumber,
                                            domain = "QBD",
                                            DueDate = invoice.Item2.DueDate,
                                            paymentDetails = driver.DriverInvoice[i].paymentDetails,
                                            TxnDate = invoice.Item2.TxnDate.ToString(),
                                            Balance = invoice.Item2.BalanceRemaining,
                                            CustomField = cf,
                                            Memo = invoice.Item2.Memo,
                                            status = status,
                                            TotalAmt = invoice.Item2.Subtotal,
                                            uniqueId = id
                                        };

                                    }
                                    else if (!addableDriverInvoices.Any(p => p.Id == invoice.Item2?.TxnID) && !driver.DriverInvoice.Any(p => p.Id == invoice.Item2?.TxnID))
                                    {

                                        addableDriverInvoices.Add(new DriverInvoice
                                        {
                                            Id = invoice.Item2.TxnID,
                                            DocNumber = invoice.Item2.RefNumber,
                                            domain = "QBD",
                                            DueDate = invoice.Item2.DueDate,
                                            paymentDetails = driver.DriverInvoice[i].paymentDetails,
                                            TxnDate = invoice.Item2.TxnDate.ToString(),
                                            Balance = invoice.Item2.BalanceRemaining,
                                            CustomField = cf,
                                            Memo = invoice.Item2.Memo,
                                            status = status,
                                            TotalAmt = invoice.Item2.Subtotal,
                                            uniqueId = ObjectId.GenerateNewId().ToString()
                                        });
                                    }

                                    // ✅ Now modification is allowed
                                }
                            }
                            else
                            {

                                addableDriverInvoices.Add(new DriverInvoice
                                {
                                    Id = invoice.Item2.TxnID,
                                    DocNumber = invoice.Item2.RefNumber,
                                    domain = "QBD",
                                    DueDate = invoice.Item2.DueDate,
                                    paymentDetails = driver.InvoicePayments.FirstOrDefault(o => o.InvoiceId == invoice.Item2?.TxnID),
                                    TxnDate = invoice.Item2.TxnDate.ToString(),
                                    Balance = invoice.Item2.BalanceRemaining,
                                    CustomField = cf,
                                    Memo = invoice.Item2.Memo,
                                    status = status,
                                    TotalAmt = invoice.Item2.Subtotal,
                                    uniqueId = ObjectId.GenerateNewId().ToString()
                                });
                            }

                            if (addableDriverInvoices?.Count > 0)
                            {
                                driver.DriverInvoice.AddRange(addableDriverInvoices);
                            }

                            if (addableinvoices?.Count > 0)
                            {
                                driver.Invoices.AddRange(addableinvoices);
                            }
                            await mongodbService.UpdateUserAsync(driver.Id, driver);
                        }

                    }
                }


                user.QuickBooks.QbwdLastSync = DateTime.UtcNow;
                await mongodbService.UpdateUserAsync(user.Id, user);
            }


        }




        public async Task<string> closeConnection(string ticket)
        {
            if (sessionTickets.ContainsKey(ticket))
            {
                var user = sessionTickets[ticket];
                if (user != null)
                {
                    //user.QuickBooks.Connected = false;
                    user.QuickBooks.QbwdLastSync = DateTime.UtcNow;
                    await mongodbService.UpdateUserAsync(user.Id, user);
                }
                sessionTickets.Remove(ticket);
            }
            return "Connection closed successfully.";
        }
    }
}
