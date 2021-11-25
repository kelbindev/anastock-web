using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class ProductBalanceDetailsViewModel
    {
        public int Qty { get; set; }
        public string Description { get; set; }
        [DisplayName("Date")]
        public DateTime CreatedDate { get; set; }
        public string InvoicePo { get; set; }
    }
}
