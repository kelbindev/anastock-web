using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Company;
        public CompanyRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        public CompanyViewModel GetCompany(int companyId)
        {
            CompanyViewModel c = context.Company.Find(companyId);
            return c;
        }

        public bool Update(CompanyViewModel model)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var c = context.Company.Where(q => q.CompanyId == model.CompanyId).FirstOrDefault();
                    c.Name = model.Name;
                    c.Address = model.Address;
                    c.Email = model.Email;
                    c.Website = model.Website;
                    c.Phone = model.Phone;
                    c.Fax = model.Fax;
                    c.IsGSTEnable = model.IsGSTEnable;
                    c.GSTRegNo = model.IsGSTEnable == true ? model.GSTRegNo : "-";
                    c.GST = model.IsGSTEnable == true ? model.GST : 0;
                    c.Logo = model.Logo == null ? c.Logo : model.Logo;
                    c.LogoExtension = model.Logo == null ? c.LogoExtension : model.LogoExtension;
                    context.Update(c);

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = new Guid(),
                        ModuleDescription = model.Name,
                        ActivityType = ActivityType.Update,
                        User = userName,
                        CompanyId = model.CompanyId
                    };
                    loggerRepository.Create(act);

                    result = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    string err = ex.Message.ToString();
                    result = false;
                    transaction.Rollback();
                    loggerRepository.saveError(ex.ToString());
                }

            }
            return result;
        }
    }
}
