using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Vendor : CommonFields
    {
        [Key]
        [Required]
        public Guid VendorId { get; set; }
        [Required, MaxLength(100)]
        public string VendorName { get; set; }
        [Required, MaxLength(200)]
        public string VendorEmail { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string Website { get; set; }
        [Required]
        public bool isActive { get; set; }
        public ICollection<VendorAddress> vendorAddresses { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<Bill> Bills { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
