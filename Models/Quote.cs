using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Quote : CommonFields
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuoteId { get; set; }

        [Display(Name = "Quote Number")]
        [Required, MaxLength(20)]
        public string QuoteNo { get; set; }

        [Display(Name = "Customer PO Number")]
        [MaxLength(20)]
        public string CustomerPONo { get; set; }

        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime ExpiryDate { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
        public bool TaxInclusive { get; set; }

        [Display(Name = "Notes for Customer")]
        [MaxLength(200)]
        public string? CustomerNotes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        [MaxLength(10)]
        [DisplayName("Discount Type")]
        public string? DiscountType { get; set; }
        [DisplayName("Discount")]
        public decimal? DiscountValue { get; set; }
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
        public bool isCurrentUse { get; set; }
        //Relationship
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int CustomerAddressId { get; set; }
        public CustomerAddress CustomerAddress { get; set; }
        public Invoice Invoice { get; set; }
        public ICollection<QuoteDetails> QuoteDetails { get; set; }
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }

        [ForeignKey("Project")]
        public Guid? LinkedProjectId { get; set; }
        public Project Project { get; set; }

    }
}
