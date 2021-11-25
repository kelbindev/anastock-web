using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class CustomerViewModel
    {
        public Guid CustomerId { get; set; }
        [Required, MaxLength(100)]
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        [Required, MaxLength(200)]
        [DisplayName("Customer Email")]
        public string CustomerEmail { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string Website { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Country")]
        public string BillingCountry { get; set; }
        [Required, MaxLength(500)]
        [DisplayName("Address")]
        public string BillingAddress { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Town")]
        public string BillingTown { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("State")]
        public string BillingState { get; set; }
        [Required, MaxLength(10)]
        [DisplayName("Postal Code")]
        public string BillingPostalCode { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Contact Person")]
        public string BillingContactPerson { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Email")]
        public string BillingContactEmail { get; set; }
        [Required, MaxLength(20)]
        [DisplayName("Fax")]
        public string BillingContactFax { get; set; }
        [Required, MaxLength(20)]
        [DisplayName("Phone 1")]
        public string BillingContactPhone1 { get; set; }
        [MaxLength(20)]
        [DisplayName("Phone 2")]
        public string BillingContactPhone2 { get; set; }
        [MaxLength(20)]
        [DisplayName("Phone 3")]
        public string BillingContactPhone3 { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Country")]
        public string ShippingCountry { get; set; }
        [Required, MaxLength(500)]
        [DisplayName("Address")]
        public string ShippingAddress { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Town")]
        public string ShippingTown { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("State")]
        public string ShippingState { get; set; }
        [Required, MaxLength(10)]
        [DisplayName("Postal Code")]
        public string ShippingPostalCode { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Contact Person")]
        public string ShippingContactPerson { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Email")]
        public string ShippingContactEmail { get; set; }
        [Required, MaxLength(20)]
        [DisplayName("Fax")]
        public string ShippingContactFax { get; set; }
        [Required, MaxLength(20)]
        [DisplayName("Phone 1s")]
        public string ShippingContactPhone1 { get; set; }
        [MaxLength(20)]
        [DisplayName("Phone 2")]
        public string ShippingContactPhone2 { get; set; }
        [MaxLength(20)]
        [DisplayName("Phone 3")]
        public string ShippingContactPhone3 { get; set; }
    }
}
