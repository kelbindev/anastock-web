using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class BillViewModels
    {
        public Guid BillId { get; set; }
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
        public string DueDate { get; set; }
        [MaxLength(20)]
        public string? Status { get; set; }
        public bool TaxInclusive { get; set; }
        [MaxLength(200)]
        [DisplayName("Notes")]
        public string? VendorNotes { get; set; }
        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        [DisplayName("Revision No")]
        public int RevisionNo { get; set; }
        [MaxLength(50)]
        [DisplayName("Payment Term")]
        public string? PaymentTerm { get; set; }
        public int? PaymentTermValue { get; set; }
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }
        [Display(Name = "Balance Due")]
        public decimal BalanceDue { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        [Display(Name = "Purchase Order")]
        public string? PurchaseOrderNo { get; set; }
    }
}
