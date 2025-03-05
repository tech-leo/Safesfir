using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safesfir.Data
{
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; } = "";
        public string Via { get; set; } = "";
        public string CompanyName    { get; set; } = "";
        public string CompanyAddressLine1 { get; set; } = "";
        public string CompanyCountry { get; set; } = "";
        public DateTime? InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } = DateTime.Now.AddDays(15);
        public string CustomerName { get; set; } = "";
        public string CustomerBillingAddressLine1 { get; set; } = "";
        public string CustomerBillingAddressLine2 { get; set; } = "";
        public string CustomerBillingCity { get; set; } = "";
        public string CustomerBillingCountry { get; set; } = "";
        public string CustomerShippingAddressLine1 { get; set; } = "";
        public string CustomerShippingCity { get; set; } = "";
        public string CustomerShippingCountry { get; set; } = "";
        public string CustomerPhoneNumber { get; set; } = "";
        public string Terms { get; set; } = "";
        public DateTime? ShipDate { get; set; } 
        public string Memo { get; set; } = "";
        public string CustomerShippingAddressLine2 { get; set; } = "";
        public List<InvoiceItem> Items { get; set; } = new();
        public decimal? TotalAmount => Items.Sum(i => i.Quantity * i.Rate);
    }

    public class InvoiceItem
    {
        public string Name { get; set; }="";
        public string Description { get; set; } = "";
        public string UOM { get; set; } = "";
        public decimal? Quantity { get; set; } = 1;
        public decimal Rate { get; set; } = 100.00m;
    }

}
