using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class ProductAndService : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [Required,MaxLength(20)]
        public string UOM { get; set; }
        [Required]
        public bool isSell { get; set; }
        public decimal? SellPrice { get; set; }
        [MaxLength(20)]
        public string? SellUOM { get; set; }
        [Required]
        public bool isPurchase { get; set; }
        public decimal? PurchasePrice { get; set; }
        [MaxLength(20)]
        public string? PurchaseUOM { get; set; }
        [Required]
        public bool isActive { get; set; }
        public decimal? SellQty { get; set; }
        public decimal? PurchaseQty { get; set; }

        //Relationship
        [Required]
        public int CategoryId { get; set; }
        public Category Category{ get; set; }
        public ICollection<QuoteDetails> QuoteDetails { get; set; }
        public ICollection<InvoiceDetails> InvoiceDetails { get; set; }
        public ICollection<PurchaseOrderDetails> PurchaseOrderDetails{ get; set; }
        public ICollection<BillDetails> BillDetails { get; set; }
        public ICollection<ProductBalanceDetails> ProductBalanceDetails { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }

    }
}
