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
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.PurchaseOrder;

        public PurchaseOrderRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public bool Convert(Guid id, int companyId)
        {
            throw new NotImplementedException();
        }

        public PurchaseOrder Create(PurchaseOrder model, int companyId)
        {
            Guid poId = Guid.NewGuid();
            PurchaseOrder newPo = new PurchaseOrder();

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var PurchaseOrder = new PurchaseOrder
                    {
                        Id = poId,
                        VendorId = model.VendorId,
                        PurchaseOrderNo = model.PurchaseOrderNo,
                        VendorInvoiceNo = model.VendorInvoiceNo,
                        IssueDate = model.IssueDate,
                        DueDate = model.DueDate,
                        Status = model.Status,
                        VendorNotes = model.VendorNotes,
                        VendorAddressId = model.VendorAddressId,
                        SubTotal = model.SubTotal,
                        Tax = model.Tax,
                        Total = model.Total,
                        CompanyId = companyId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now,
                        PaymentTerm = model.PaymentTerm,
                        PaymentTermValue = model.PaymentTermValue,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue
                    };
                    context.PurchaseOrders.Add(PurchaseOrder);

                    foreach (var detail in model.purchaseOrdersDetails)
                    {
                        var PurchaseOrderDetail = new PurchaseOrderDetails
                        {
                            PurchaseOrderId = poId,
                            ProductAndServiceId = detail.ProductAndServiceId,
                            Qty = detail.Qty,
                            UnitPrice = detail.UnitPrice,
                            DiscountPercent = detail.DiscountPercent,
                            Total = detail.Total,
                            Description = detail.Description,
                            UOM = detail.UOM
                        };
                        context.PurchaseOrderDetails.Add(PurchaseOrderDetail);
                        context.SaveChanges();
                    }

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = poId,
                        ModuleDescription = model.PurchaseOrderNo,
                        ActivityType = ActivityType.Create,
                        User = userName,
                        CompanyId = companyId
                    };
                    loggerRepository.Create(act);

                    newPo = PurchaseOrder;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    string err = ex.Message.ToString();
                    transaction.Rollback();
                    loggerRepository.saveError(ex.ToString());
                }

            }
            return newPo;
        }

        public bool Delete(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    PurchaseOrder po = context.PurchaseOrders.Where(q => q.Id == id).FirstOrDefault();
                    po.IsDeleted = true;
                    po.UpdatedBy = userName;
                    context.Update(po);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = po.PurchaseOrderNo,
                        ActivityType = ActivityType.Delete,
                        User = userName,
                        CompanyId = companyId
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

        public bool CancelPO(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    PurchaseOrder po = context.PurchaseOrders.Where(q => q.Id == id).FirstOrDefault();
                    po.Status = "Cancelled";
                    po.UpdatedBy = userName;
                    context.Update(po);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = po.PurchaseOrderNo,
                        ActivityType = ActivityType.Cancel,
                        User = userName,
                        CompanyId = companyId
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

        public IEnumerable<PurchaseOrder> GetAllPurchaseOrders()
        {
            return context.PurchaseOrders;
        }

        public VendorAddress getVendorAddress(int id)
        {
            VendorAddress address = context.VendorAddresses.SingleOrDefault(
              a => a.VendorAddressId == id
          );
            return address;
        }

        public PurchaseOrder GetPurchaseOrder(Guid id)
        {
            PurchaseOrder po = context.PurchaseOrders.Find(id);
            return po;
        }

        public IEnumerable<PurchaseOrderDetails> GetPurchaseOrderDetails(Guid id)
        {
            IEnumerable<PurchaseOrderDetails> po = context.PurchaseOrderDetails.Where(q => q.PurchaseOrderId == id);
            return po;
        }

        public IEnumerable<PurchaseOrder> GetPurchaseOrdersByCompanyId(int id)
        {
            IEnumerable<PurchaseOrder> po = context.PurchaseOrders.Where(
                q => q.CompanyId == id && q.IsDeleted == false
            );
            return po;
        }

        public bool Update(PurchaseOrder model, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var PurchaseOrder = context.PurchaseOrders.Where(q => q.Id == model.Id).FirstOrDefault();
                    PurchaseOrder.VendorId = model.VendorId;
                    PurchaseOrder.PurchaseOrderNo = model.PurchaseOrderNo;
                    PurchaseOrder.VendorInvoiceNo = model.VendorInvoiceNo;
                    PurchaseOrder.IssueDate = model.IssueDate;
                    PurchaseOrder.DueDate = model.DueDate;
                    PurchaseOrder.Status = model.Status;
                    PurchaseOrder.VendorNotes = model.VendorNotes;
                    PurchaseOrder.VendorAddressId = model.VendorAddressId;
                    PurchaseOrder.SubTotal = model.SubTotal;
                    PurchaseOrder.Tax = model.Tax;
                    PurchaseOrder.Total = model.Total;
                    PurchaseOrder.CompanyId = companyId;
                    PurchaseOrder.UpdatedBy = userName;
                    PurchaseOrder.PaymentTerm = model.PaymentTerm;
                    PurchaseOrder.PaymentTermValue = model.PaymentTermValue;
                    PurchaseOrder.DiscountType = model.DiscountType;
                    PurchaseOrder.DiscountValue = model.DiscountValue;
                    context.Update(PurchaseOrder);

                    var listdetail = context.PurchaseOrderDetails.Where(pod => pod.PurchaseOrderId == model.Id);

                    List<int> detList = new List<int>();
                    foreach (var l in listdetail)
                    {
                        detList.Add(l.PurchaseOrderDetailsId);
                    }

                    foreach (var detail in model.purchaseOrdersDetails)
                    {
                        var PurchaseOrderDetail = context.PurchaseOrderDetails.Where(qd => qd.PurchaseOrderDetailsId == detail.PurchaseOrderDetailsId).FirstOrDefault();
                        if (PurchaseOrderDetail != null)
                        {
                            detList.Remove(PurchaseOrderDetail.PurchaseOrderDetailsId);
                            PurchaseOrderDetail.PurchaseOrderId = detail.PurchaseOrderId;
                            PurchaseOrderDetail.ProductAndServiceId = detail.ProductAndServiceId;
                            PurchaseOrderDetail.Qty = detail.Qty;
                            PurchaseOrderDetail.UnitPrice = detail.UnitPrice;
                            PurchaseOrderDetail.DiscountPercent = detail.DiscountPercent;
                            PurchaseOrderDetail.Total = detail.Total;
                            PurchaseOrderDetail.Description = detail.Description;
                            PurchaseOrderDetail.UOM = detail.UOM;
                            PurchaseOrderDetail.DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100;
                            context.Update(PurchaseOrderDetail);
                        }
                        else
                        {
                            var newPurchaseOrderDetail = new PurchaseOrderDetails
                            {
                                PurchaseOrderId = detail.PurchaseOrderId,
                                ProductAndServiceId = detail.ProductAndServiceId,
                                Qty = detail.Qty,
                                UnitPrice = detail.UnitPrice,
                                DiscountPercent = detail.DiscountPercent,
                                Total = detail.Total,
                                Description = detail.Description,
                                UOM = detail.UOM,
                                DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100
                            };
                            context.PurchaseOrderDetails.Add(newPurchaseOrderDetail);
                        }
                    }
                    foreach (int i in detList)
                    {
                        PurchaseOrderDetails q = context.PurchaseOrderDetails.Find(i);
                        context.PurchaseOrderDetails.Remove(q);
                    }
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = model.Id,
                        ModuleDescription = model.PurchaseOrderNo,
                        ActivityType = ActivityType.Update,
                        User = userName,
                        CompanyId = companyId
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
