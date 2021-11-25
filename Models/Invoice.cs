using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Invoice : CommonFields
    {
        [Key]
        [Required]
        public Guid InvoiceId { get; set; }
        [Display(Name = "Invoice No")]
        [Required,MaxLength(20)]
        public string InvoiceNo { get; set; }
        [Display(Name = "Customer PO No")]
        [MaxLength(20)]
        public string? CustomerPONo { get; set; }
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime IssueDate { get; set; }
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }
        [MaxLength(20)]
        public string? Status { get; set; }
        public bool TaxInclusive { get; set; }
        [Display(Name = "Customer Notes")]
        [MaxLength(200)]
        public string? CustomerNotes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? Tax { get; set; }
        [MaxLength(10)]
        [DisplayName("Discount Type")]
        public string? DiscountType { get; set; }
        [DisplayName("Discount")]
        public decimal? DiscountValue { get; set; }

        [Display(Name = "Amount")]
        public decimal Total { get; set; }
        public int RevisionNo { get; set; }
        [MaxLength(50)]
        public string? CreditTerm { get; set; }
        [MaxLength(50)]
        public string? ShippingTerm { get; set; }
        [MaxLength(50)]
        public string? DeliveryTerm { get; set; }
        [MaxLength(50)]
        public string? PaymentTerm { get; set; }
        public int? PaymentTermValue { get; set; }
        public bool isCurrentUse { get; set; }

        //Relationship
        public Guid? LinkedQuoteId { get; set; }
        public Quote LinkedQuote { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int CustomerAddressId { get; set; }
        public CustomerAddress CustomerAddress { get; set; }
        public ICollection<InvoiceDetails> invoiceDetails { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }
        [Display(Name = "Balance Due")]
        public decimal BalanceDue { get; set; }
        public ICollection<InvoiceReceivable> invoiceReceivables { get; set; }
        public ICollection<ProductBalanceDetails> ProductBalanceDetails { get; set; }
    }
}
