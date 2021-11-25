using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace Anastock.Models
{
    public class Bill : BaseModel
    {
        [Required, MaxLength(20)]
        [DisplayName("Bill No")]
        public string BillNo { get; set; }
        [MaxLength(20)]
        [DisplayName("Vendor Invoice No")]
        public string? VendorInvoiceNo { get; set; }
        [Required]
        [DisplayName("Issue Date")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }
        [Required]
        [DisplayName("Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        [MaxLength(20)]
        public string? Status { get; set; }
        public bool TaxInclusive { get; set; }
        [MaxLength(200)]
        [DisplayName("Notes")]
        public string? VendorNotes { get; set; }
        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }
        public decimal? Tax { get; set; }
        [MaxLength(10)]
        [DisplayName("Discount Type")]
        public string? DiscountType { get; set; }
        [DisplayName("Discount")]
        public decimal? DiscountValue { get; set; }
        [DisplayName("Total")]
        public decimal Total { get; set; }
        [DisplayName("Revision No")]
        public int RevisionNo { get; set; }
        [MaxLength(50)]
        [DisplayName("Credit Term")]
        public string? CreditTerm { get; set; }
        [MaxLength(50)]
        [DisplayName("Shipping Term")]
        public string? ShippingTerm { get; set; }
        [MaxLength(50)]
        [DisplayName("Delivery Term")]
        public string? DeliveryTerm { get; set; }
        [MaxLength(50)]
        [DisplayName("Payment Term")]
        public string? PaymentTerm { get; set; }
        public int? PaymentTermValue { get; set; }
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }
        [Display(Name = "Balance Due")]
        public decimal BalanceDue { get; set; }
        public bool isCurrentUse { get; set; }

        //Relationships
        [ForeignKey("LinkedPurchaseOrder")]
        public Guid? LinkedPOId { get; set; }
        public PurchaseOrder LinkedPurchaseOrder { get; set; }
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int VendorAddressId { get; set; }
        public VendorAddress VendorAddress { get; set; }
        public ICollection<BillDetails> BillDetails { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
        public ICollection<BillPayment> BillPayments{ get; set; }
        public ICollection<ProductBalanceDetails> ProductBalanceDetails { get; set; }
    }
}
