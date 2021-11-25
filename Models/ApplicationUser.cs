using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
        public ICollection<Claim> Claim { get; set; }
    }
}
