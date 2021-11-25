using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class Category
    {
        [Key]
        public int CateogryId { get; set; }
        public string CategoryName { get; set; }
        //Relationship
        public ICollection<ProductAndService> ProductAndServices { get; set; }
    }
}
