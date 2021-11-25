using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class InvoiceReceivableViewModel
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid InvoiceId { get; set; }
        [Required]
        public Guid PaymentMethodId { get; set; }
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Reference Number")]
        [Required]
        public string ReferenceNo { get; set; }
        [Display(Name = "Description")]
        [MaxLength(250)]
        public string Description { get; set; }
        [Display(Name = "Amount Received")]
        [Required]
        public decimal AmountReceived { get; set; }

        [Display(Name = "Payment Type")]
        public string? PaymentName { get; set; }
        [Display(Name = "Invoice Number")]
        public string? InvoiceNo { get; set; }

    }
}
