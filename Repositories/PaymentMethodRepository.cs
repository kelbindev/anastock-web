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
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.PaymentMethod;
        public PaymentMethodRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        public bool Create(PaymentMethod NewPaymentMethod)
        {
            bool result = true;
            Guid pmid = Guid.NewGuid();
            if (NewPaymentMethod != null)
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    try
                    {
                        PaymentMethod pm = new PaymentMethod
                        {
                            PaymentMethodId = pmid,
                            Description = NewPaymentMethod.Description,
                            AccountNumber = NewPaymentMethod.AccountNumber,
                            CompanyId = NewPaymentMethod.CompanyId,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false
                        };
                        context.PaymentMethods.Add(pm);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = pmid,
                            ModuleDescription = NewPaymentMethod.Description,
                            ActivityType = ActivityType.Create,
                            User = userName,
                            CompanyId = NewPaymentMethod.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        result = false;
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }

        public bool Delete(Guid id)
        {
            bool result = true;
            if (id != Guid.Empty)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var pm = context.PaymentMethods.Where(p => p.PaymentMethodId == id).FirstOrDefault();
                        pm.IsDeleted = true;
                        context.Update(pm);

                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = id,
                            ModuleDescription = pm.Description,
                            ActivityType = ActivityType.Delete,
                            User = userName,
                            CompanyId = pm.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        result = false;
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }

        public IEnumerable<PaymentMethod> GetAllPaymentMethod(int companyId)
        {
            return context.PaymentMethods
                .Where(x => x.IsDeleted == false && x.CompanyId == companyId);
        }

        public PaymentMethod GetPaymentMethod(Guid id)
        {
            PaymentMethod pm = context.PaymentMethods.Find(id);
            return pm;
        }

        public bool Update(PaymentMethod PaymentMethodChanges)
        {
            bool result = true;
            if (PaymentMethodChanges != null)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var pm = context.PaymentMethods.Where(v => v.PaymentMethodId == PaymentMethodChanges.PaymentMethodId).FirstOrDefault();

                        pm.Description = PaymentMethodChanges.Description;
                        pm.AccountNumber = PaymentMethodChanges.AccountNumber;
                        pm.UpdatedBy = userName;
                        pm.UpdatedDate = DateTime.Now;
                        context.Update(pm);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = PaymentMethodChanges.PaymentMethodId,
                            ModuleDescription = PaymentMethodChanges.Description,
                            ActivityType = ActivityType.Update,
                            User = userName,
                            CompanyId = pm.CompanyId
                        };
                        loggerRepository.Create(act);


                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        result = false;
                        loggerRepository.saveError(ex.ToString());
                    }
                }
            }
            return result;
        }
    }
}
