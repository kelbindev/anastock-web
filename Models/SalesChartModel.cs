using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class SalesChartModel
    {
        public string Period { get; set; }
        public decimal Total { get; set; }
        public decimal BalanceDue { get; set; }
    }
}
