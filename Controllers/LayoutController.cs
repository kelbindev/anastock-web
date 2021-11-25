using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anastock.Controllers
{
    public class LayoutController : Controller
    {
        private readonly AnastockContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public LayoutController(AnastockContext context, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            this.context = context;
        }
        public IActionResult ValidateCompany()
        {
            bool valid = true;
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var companyId = users.CompanyId;
            var company = context.Company.Where(c => c.CompanyId == companyId).FirstOrDefault();
            if (company != null)
            {
                if (string.IsNullOrEmpty(company.Name))
                {
                    valid = false;
                }
                if (string.IsNullOrEmpty(company.Address))
                {
                    valid = false;
                }
                if (string.IsNullOrEmpty(company.Email))
                {
                    valid = false;
                }
                if (string.IsNullOrEmpty(company.Phone))
                {
                    valid = false;
                }
            }
            return Ok(valid);
        }
    }
}
