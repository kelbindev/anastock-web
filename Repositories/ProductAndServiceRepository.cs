using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class ProductAndServiceRepository : IProductAndServiceRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.ProductAndService;
        //private readonly ILog logger;
        public ProductAndServiceRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public bool Create(ProductAndServiceViewModel ProductAndServiceInfo)
        {
            bool result = true;
            Guid psId = Guid.NewGuid();
            if (ProductAndServiceInfo != null)
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    string userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                    try
                    {
                        ProductAndService ps = new ProductAndService
                        {
                            Id = psId,
                            Name = ProductAndServiceInfo.Name,
                            isSell = ProductAndServiceInfo.isSell,
                            SellPrice = ProductAndServiceInfo.SellPrice,
                            SellUOM = ProductAndServiceInfo.SellUOM,
                            isPurchase = ProductAndServiceInfo.isPurchase,
                            PurchasePrice = ProductAndServiceInfo.PurchasePrice,
                            PurchaseUOM = ProductAndServiceInfo.PurchaseUOM,
                            CategoryId = ProductAndServiceInfo.CategoryId,
                            CompanyId = ProductAndServiceInfo.CompanyId,
                            CreatedBy = userName,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = userName,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            isActive = ProductAndServiceInfo.isActive,
                            PurchaseQty = ProductAndServiceInfo.PurchaseQty,
                            SellQty = ProductAndServiceInfo.SellQty,
                            UOM = ProductAndServiceInfo.UOM
                        };
                        context.ProductAndService.Add(ps);
                        
                        if (ps.CategoryId == 1)
                        {
                            ProductBalance pb = new ProductBalance
                            {
                                ProductId = ps.Id,
                                Balance = 0
                            };

                            context.productBalances.Add(pb);
                        }
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = psId,
                            ModuleDescription = ProductAndServiceInfo.Name,
                            ActivityType = ActivityType.Create,
                            User = userName,
                            CompanyId = ProductAndServiceInfo.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        loggerRepository.saveError(ex.ToString());
                        result = false;
                    }
                }
            }
            else
            {
                loggerRepository.saveError("NULL");
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
                        var ps = context.ProductAndService.Where(p => p.Id == id).FirstOrDefault();
                        ps.IsDeleted = true;
                        context.Update(ps);

                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = id,
                            ModuleDescription = ps.Name,
                            ActivityType = ActivityType.Delete,
                            User = userName,
                            CompanyId = ps.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        loggerRepository.saveError(ex.ToString());
                        result = false;
                    }
                }
            }
            return result;
        }

        public IEnumerable<ProductAndService> GetAllProductAndService(int companyId)
        {
            return context.ProductAndService
                .Where(x => x.IsDeleted == false && x.CompanyId == companyId);
        }

        public IEnumerable<ProductAndService> GetProductAndServiceById(int companyId, Guid id)
        {
            return context.ProductAndService
                .Where(x => x.IsDeleted == false && x.CompanyId == companyId && x.Id == id);
        }

        public ProductAndService GetProductAndService(Guid id)
        {
            ProductAndService ps = context.ProductAndService.Find(id);
            return ps;
        }

        public bool Update(ProductAndServiceViewModel ProductAndServiceInfo)
        {
            bool result = true;
            if (ProductAndServiceInfo != null)
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                    try
                    {
                        var ps = context.ProductAndService.Where(v => v.Id == ProductAndServiceInfo.ProductAndServiceId).FirstOrDefault();

                        ps.Name = ProductAndServiceInfo.Name;
                        ps.isSell = ProductAndServiceInfo.isSell;
                        ps.SellPrice = ProductAndServiceInfo.isSell ? ProductAndServiceInfo.SellPrice : null;
                        ps.SellUOM = ProductAndServiceInfo.isSell ? ProductAndServiceInfo.SellUOM : null;
                        ps.isPurchase = ProductAndServiceInfo.isPurchase;
                        ps.PurchasePrice = ProductAndServiceInfo.isPurchase ? ProductAndServiceInfo.PurchasePrice : null;
                        ps.PurchaseUOM = ProductAndServiceInfo.isPurchase ? ProductAndServiceInfo.PurchaseUOM : null;
                        ps.CategoryId = ProductAndServiceInfo.CategoryId;
                        ps.UpdatedBy = userName;
                        ps.UpdatedDate = DateTime.Now;
                        ps.SellQty = ProductAndServiceInfo.SellQty;
                        ps.PurchaseQty = ProductAndServiceInfo.PurchaseQty;
                        ps.UOM = ProductAndServiceInfo.UOM;
                        context.Update(ps);
                        context.SaveChanges();

                        Activity act = new Activity
                        {
                            Modules = module,
                            ModuleId = ProductAndServiceInfo.ProductAndServiceId,
                            ModuleDescription = ProductAndServiceInfo.Name,
                            ActivityType = ActivityType.Update,
                            User = userName,
                            CompanyId = ps.CompanyId
                        };
                        loggerRepository.Create(act);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        loggerRepository.saveError(ex.ToString());
                        dbContextTransaction.Rollback();
                        result = false;
                    }
                }
            }

            return result;
        }

        public IEnumerable<ProductBalanceViewModel> getStockBalance(int companyId)
        {
            var balance = from p in context.ProductAndService
                          join pb in context.productBalances
                          on p.Id equals pb.ProductId
                          where p.CompanyId == companyId
                          select new ProductBalanceViewModel
                          {
                              ProductId = p.Id,
                              ProductName = p.Name,
                              Balance = pb.Balance
                          };

            return balance;
        }

        public IEnumerable<ProductBalanceDetailsViewModel> getStockBalanceDetails(Guid productId)
        {
            var result
                 = from pbd in context.ProductBalanceDetails
                   join i in context.Invoices on pbd.LinkedInvoiceId equals i.InvoiceId into join1
                   from grp1 in join1.DefaultIfEmpty()
                   join b in context.Bills on pbd.LinkedBillId equals b.Id into join2
                   from grp2 in join2.DefaultIfEmpty()
                   where pbd.ProductId == productId
                   select new ProductBalanceDetailsViewModel
                   {
                       Qty = grp1.InvoiceNo == null ? Convert.ToInt32(pbd.Qty) : Convert.ToInt32(pbd.Qty) * -1,
                       Description = pbd.Description,
                       CreatedDate = pbd.CreatedDate,
                       InvoicePo = grp1.InvoiceNo == null ?
                      "Bill: " + grp2.BillNo
                      : "Invoice: " + grp1.InvoiceNo
                   };

            return result.OrderByDescending(x => x.CreatedDate);
        }

    }
}
