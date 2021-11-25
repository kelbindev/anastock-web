using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class ProductBalanceViewModel
    {
        public Guid ProductId { get; set; }
        [DisplayName("Product")]
        public string ProductName { get; set; }
        public decimal Balance { get; set; }
    }
}
