using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class ProductBalance
    {
        [Key]
        public Guid ProductId { get; set; }
        public Decimal Balance { get; set; } 

        //Relationship
        public ProductAndService ProductAndService { get; set; }
    }
}
