using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    [Table("Quotes")]
    public class QuotationViewModel
    {
        public int QuoteId { get; set; }
        public string QuoteNo { get; set; }
        public string CustomerPONo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
        public bool TaxInclusive { get; set; }
        public string CustomerNotes { get; set; }


    }
}
