using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class InvoiceDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int InvoiceDetailsId { get; set; }
        [MaxLength(20)]
        public string UOM { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountTotal { get; set; }
        public decimal Total { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        //Relationship
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public Guid ProductAndServiceId { get; set; }
        public ProductAndService ProductService { get; set; }
    }
}
