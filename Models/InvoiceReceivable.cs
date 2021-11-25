using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class InvoiceReceivable : CommonFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int InvoiceReceivableId { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Reference number")]
        [Required]
        public string ReferenceNumber { get; set; }
        [MaxLength(250)]
        [Display(Name = "Description")]
        public string? DescriptionOfTransaction { get; set; }
        [Display(Name = "Amount Received")]
        public decimal AmountReceived { get; set; }
        //Relationship
        public Guid PaymentMethodId { get; set; }
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        public Guid LinkedInvoiceId { get; set; }
        [Display(Name = "Invoice number")]
        public Invoice LinkedInvoice { get; set; }
    }
}
