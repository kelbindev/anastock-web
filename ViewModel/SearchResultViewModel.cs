using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class SearchResultViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNo { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public Decimal Total { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public decimal? SellPrice { get; set; }
        public string? SellUOM { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? PurchaseUOM { get; set; }
        public string ItemType { get; set; }
        public string CustomerName { get; set; }
        public string VendorName { get; set; }
        public string Url { get; set; }
        public List<ItemDetailViewModel> ItemDetail { get; set; }
    }
}
public class ItemDetailViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Qty { get; set; }
    public string UOM { get; set; }
    public decimal Price { get; set; }
    public decimal? Percentage { get; set; }
    public decimal Total { get; set; }
}