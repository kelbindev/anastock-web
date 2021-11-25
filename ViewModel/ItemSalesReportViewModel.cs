using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class ItemSalesReportViewModel
    {
        public string Name { get; set; }
        public string UOM { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
    }
}
