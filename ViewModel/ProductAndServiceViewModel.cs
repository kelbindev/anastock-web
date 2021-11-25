using Anastock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class ProductAndServiceViewModel
    {
        public Guid ProductAndServiceId { get; set; }
        [Display(Name = "Product/Service Name")]
        [Required]
        public string Name { get; set; }
        public string UOM { get; set; }
        public bool isSell { get; set; }
        public decimal? SellPrice { get; set; }
        [MaxLength(20)]
        public string? SellUOM { get; set; }
        public bool isPurchase { get; set; }
        public decimal? PurchasePrice { get; set; }
        [MaxLength(20)]
        public string? PurchaseUOM { get; set; }
        public bool isActive { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public decimal? SellQty { get; set; }
        public decimal? PurchaseQty { get; set; }
    }
}
