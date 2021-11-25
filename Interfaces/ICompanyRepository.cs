using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface ICompanyRepository
    {
        CompanyViewModel GetCompany(int companyId);
        bool Update(CompanyViewModel model);
    }
}
