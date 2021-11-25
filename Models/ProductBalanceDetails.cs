using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class ProductBalanceDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public Decimal Qty { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        //Relationship
        public Guid ProductId { get; set; }
        public ProductAndService ProductService { get; set; }
        public Guid? LinkedBillId { get; set; }
        public Bill Bill { get; set; }
        public Guid? LinkedInvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
