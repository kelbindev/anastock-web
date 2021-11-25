using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.ViewModel
{
    public class BillPaymentViewModel
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid BillId { get; set; }
        [Required]
        public Guid PaymentMethodId { get; set; }
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Reference Number")]
        public string? ReferenceNo { get; set; }
        [Display(Name = "Description")]
        [MaxLength(250)]
        public string Description { get; set; }
        [Display(Name = "Amount Received")]
        [Required]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Payment Type")]
        public string? PaymentName { get; set; }
        [Display(Name = "Bill Number")]
        public string? BillNo { get; set; }
    }
}
