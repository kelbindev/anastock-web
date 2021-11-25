using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class VendorAddress : CommonFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int VendorAddressId { get; set; }
        [MaxLength(50)]
        public string BillingCountry { get; set; }
        [MaxLength(500)]
        public string BillingAddress { get; set; }
        [MaxLength(50)]
        public string BillingTown { get; set; }
        [MaxLength(50)]
        public string BillingState { get; set; }
        [MaxLength(10)]
        public string BillingPostalCode { get; set; }
        [MaxLength(50)]
        public string BillingContactPerson { get; set; }
        [MaxLength(50)]
        public string BillingContactEmail { get; set; }
        [MaxLength(20)]
        public string BillingContactFax { get; set; }
        [MaxLength(20)]
        public string BillingContactPhone1 { get; set; }
        [MaxLength(20)]
        public string BillingContactPhone2 { get; set; }
        [MaxLength(20)]
        public string BillingContactPhone3 { get; set; }
        [MaxLength(50)]
        public string ShippingCountry { get; set; }
        [MaxLength(500)]
        public string ShippingAddress { get; set; }
        [MaxLength(50)]
        public string ShippingTown { get; set; }
        [MaxLength(50)]
        public string ShippingState { get; set; }
        [MaxLength(10)]
        public string ShippingPostalCode { get; set; }
        [MaxLength(50)]
        public string ShippingContactPerson { get; set; }
        [MaxLength(50)]
        public string ShippingContactEmail { get; set; }
        [MaxLength(20)]
        public string ShippingContactFax { get; set; }
        [MaxLength(20)]
        public string ShippingContactPhone1 { get; set; }
        [MaxLength(20)]
        public string ShippingContactPhone2 { get; set; }
        [MaxLength(20)]
        public string ShippingContactPhone3 { get; set; }

        [Required]
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders{ get; set; }
    }
}
