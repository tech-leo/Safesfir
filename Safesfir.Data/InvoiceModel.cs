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
        public DateTime? InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } = DateTime.Now.AddDays(15);
        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public List<InvoiceItem> Items { get; set; } = new();
        public decimal? TotalAmount => Items.Sum(i => i.Quantity * i.Rate);
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = "Service";
        public decimal? Quantity { get; set; } = 1;
        public decimal Rate { get; set; } = 100.00m;
    }

}
