using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Bill;

        public BillRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public Bill Create(Bill model, int companyId)
        {
            Guid billId = Guid.NewGuid();
            Bill newBill = new Bill();

            if (model.LinkedPOId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                model.LinkedPOId = null;
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var Bill = new Bill
                    {
                        Id = billId,
                        VendorId = model.VendorId,
                        BillNo = model.BillNo,
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
                        LinkedPOId = model.LinkedPOId,
                        BalanceDue = model.BalanceDue,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue
                    };
                    context.Bills.Add(Bill);

                    foreach (var detail in model.BillDetails)
                    {
                        var BillDetail = new BillDetails
                        {
                            BillId = billId,
                            ProductAndServiceId = detail.ProductAndServiceId,
                            Qty = detail.Qty,
                            UnitPrice = detail.UnitPrice,
                            DiscountPercent = detail.DiscountPercent,
                            Total = detail.Total,
                            Description = detail.Description,
                            UOM = detail.UOM
                        };
                        context.billDetails.Add(BillDetail);
                        context.SaveChanges();
                    }

                    if (model.LinkedPOId != Guid.Empty && model.LinkedPOId != null)
                    {
                        PurchaseOrder po = context.PurchaseOrders.Where(po => po.Id == model.LinkedPOId).FirstOrDefault();
                        po.Status = "Closed";
                        po.UpdatedBy = userName;
                        context.Update(po);
                        context.SaveChanges();
                    }

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = billId,
                        ModuleDescription = model.BillNo,
                        ActivityType = ActivityType.Create,
                        User = userName,
                        CompanyId = companyId
                    };
                    loggerRepository.Create(act);

                    newBill = Bill;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string err = ex.Message.ToString();
                    loggerRepository.saveError(ex.ToString());
                }

            }
            return newBill;
        }

        public bool Delete(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    Bill po = context.Bills.Where(q => q.Id == id).FirstOrDefault();
                    po.IsDeleted = true;
                    po.UpdatedBy = userName;
                    context.Update(po);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = po.BillNo,
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

        public IEnumerable<Bill> GetAllBills()
        {
            return context.Bills;
        }

        public VendorAddress getVendorAddress(int id)
        {
            VendorAddress address = context.VendorAddresses.SingleOrDefault(
              a => a.VendorAddressId == id
          );
            return address;
        }

        public Bill GetBill(Guid id)
        {
            Bill po = context.Bills.Find(id);
            return po;
        }

        public IEnumerable<BillDetails> GetBillDetails(Guid id)
        {
            IEnumerable<BillDetails> po = context.billDetails.Where(q => q.BillId == id);
            return po;
        }

        public IEnumerable<Bill> GetBillsByCompanyId(int Companyid)
        {
            IEnumerable<Bill> bill = context.Bills.Where(
                q => q.CompanyId == Companyid && q.IsDeleted == false
            );
            return bill;
        }

        public IEnumerable<BillViewModels> GetBillsWithPOByCompanyId(int Companyid)
        {
            IEnumerable<BillViewModels> bill =
               from b in context.Bills
                join po in context.PurchaseOrders on b.LinkedPOId equals po.Id into grp1
                from join1 in grp1.DefaultIfEmpty()
                where b.IsDeleted == false
                select new BillViewModels
                {
                    BillId = b.Id,
                    BillNo = b.BillNo,
                    VendorInvoiceNo = b.VendorInvoiceNo,
                    IssueDate = b.IssueDate,
                    DueDate = b.PaymentTerm == "Days" ? b.DueDate.ToString("MM/dd/yyyy") : b.PaymentTerm,
                    Status = b.Status,
                    TaxInclusive = b.TaxInclusive,
                    VendorNotes = b.VendorNotes,
                    SubTotal = b.SubTotal,
                    Total = b.Total,
                    RevisionNo = b.RevisionNo,
                    PaymentTerm = b.PaymentTerm,
                    PaymentTermValue = b.PaymentTermValue,
                    AmountPaid = b.AmountPaid,
                    BalanceDue = b.BalanceDue,
                    PurchaseOrderNo = join1.PurchaseOrderNo,
                    PurchaseOrderId = join1.Id
                };

            return bill.ToList();
        }

        public bool Update(Bill model, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var Bill = context.Bills.Where(q => q.Id == model.Id).FirstOrDefault();
                    Bill.VendorId = model.VendorId;
                    Bill.BillNo = model.BillNo;
                    Bill.VendorInvoiceNo = model.VendorInvoiceNo;
                    Bill.IssueDate = model.IssueDate;
                    Bill.DueDate = model.DueDate;
                    Bill.Status = model.Status;
                    Bill.VendorNotes = model.VendorNotes;
                    Bill.VendorAddressId = model.VendorAddressId;
                    Bill.SubTotal = model.SubTotal;
                    Bill.Tax = model.Tax;
                    Bill.Total = model.Total;
                    Bill.CompanyId = companyId;
                    Bill.UpdatedBy = userName;
                    Bill.PaymentTerm = model.PaymentTerm;
                    Bill.PaymentTermValue = model.PaymentTermValue;
                    Bill.BalanceDue = model.BalanceDue;
                    Bill.DiscountType = model.DiscountType;
                    Bill.DiscountValue = model.DiscountValue;
                    context.Update(Bill);

                    var listdetail = context.billDetails.Where(pod => pod.BillId == model.Id);

                    List<int> detList = new List<int>();
                    foreach (var l in listdetail)
                    {
                        detList.Add(l.BillDetailsId);
                    }

                    foreach (var detail in model.BillDetails)
                    {
                        var BillDetail = context.billDetails.Where(qd => qd.BillDetailsId == detail.BillDetailsId).FirstOrDefault();
                        if (BillDetail != null)
                        {
                            detList.Remove(BillDetail.BillDetailsId);
                            BillDetail.BillId = detail.BillId;
                            BillDetail.ProductAndServiceId = detail.ProductAndServiceId;
                            BillDetail.Qty = detail.Qty;
                            BillDetail.UnitPrice = detail.UnitPrice;
                            BillDetail.DiscountPercent = detail.DiscountPercent;
                            BillDetail.Total = detail.Total;
                            BillDetail.Description = detail.Description;
                            BillDetail.UOM = detail.UOM;
                            BillDetail.DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100;
                            context.Update(BillDetail);
                        }
                        else
                        {
                            var newBillDetail = new BillDetails
                            {
                                BillId = detail.BillId,
                                ProductAndServiceId = detail.ProductAndServiceId,
                                Qty = detail.Qty,
                                UnitPrice = detail.UnitPrice,
                                DiscountPercent = detail.DiscountPercent,
                                Total = detail.Total,
                                Description = detail.Description,
                                UOM = detail.UOM,
                                DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100
                            };
                            context.billDetails.Add(newBillDetail);
                        }
                    }
                    foreach (int i in detList)
                    {
                        BillDetails q = context.billDetails.Find(i);
                        context.billDetails.Remove(q);
                    }
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = model.Id,
                        ModuleDescription = model.BillNo,
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

        public bool CreatePayment(BillPaymentViewModel model, int companyId)
        {
            bool result = false;
            Guid qId = Guid.NewGuid();


            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                var Bill = context.Bills.Where(i => i.Id == model.BillId).FirstOrDefault();
               
                var newBalanceDue = Bill.BalanceDue - model.AmountPaid;
                var newAmountPaid = Bill.AmountPaid + model.AmountPaid;
                string status = (newBalanceDue <= 0) ? "Closed" : "On Progress";
                try
                {
                    var bill = new BillPayment
                    {
                        PaymentDate = model.PaymentDate,
                        ReferenceNumber = model.ReferenceNo,
                        DescriptionOfTransaction = model.Description,
                        AmountPaid = model.AmountPaid,
                        PaymentMethodId = model.PaymentMethodId,
                        LinkedBillId = model.BillId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now
                    };
                    context.BillPayments.Add(bill);

                    Bill.BalanceDue = newBalanceDue;
                    context.Entry(Bill).Property("BalanceDue").IsModified = true;
                    Bill.AmountPaid = newAmountPaid;
                    context.Entry(Bill).Property("AmountPaid").IsModified = true;
                    Bill.Status = status;
                    context.Entry(Bill).Property("Status").IsModified = true;

                    //Update Product Balance
                    if (status == "Closed")
                    {
                        List<BillDetails> bd = context.billDetails.Where(b => b.BillId == bill.LinkedBillId).ToList();

                        foreach (var det in bd)
                        {
                            ProductAndService p = context.ProductAndService.Where(x => x.Id == det.ProductAndServiceId).FirstOrDefault();
                            if (p.CategoryId == 1)
                            {
                                ProductBalance pb = context.productBalances.Where(x => x.ProductId == det.ProductAndServiceId).FirstOrDefault();
                                if (pb != null)
                                {
                                    decimal qty = det.Qty;

                                    if (p.PurchaseQty > 1)
                                    {
                                        qty = qty * p.PurchaseQty.Value;
                                    }

                                    pb.Balance += qty;
                                    context.Update(pb);

                                    //Add to product balance details
                                    ProductBalanceDetails pbd = new ProductBalanceDetails
                                    {
                                        ProductId = pb.ProductId,
                                        Qty = Convert.ToInt32(qty),
                                        CreatedDate = DateTime.Now,
                                        Description =
                                        String.Format(" +{0}/{2} from Bill {1}",
                                        qty.ToString("N0") + " " + p.UOM,
                                        Bill.BillNo,
                                        det.Qty.ToString("N0") + " " + det.UOM),
                                        LinkedBillId = Bill.Id
                                    };
                                    context.ProductBalanceDetails.Add(pbd);
                                }
                            }
                        }
                    }

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = Modules.BillPayment,
                        ModuleId = model.BillId,
                        ModuleDescription = Bill.BillNo,
                        ActivityType = ActivityType.Create,
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

        public List<BillPaymentViewModel> GetBillPayments(int companyId)
        {
            List<BillPaymentViewModel> billPayment = new List<BillPaymentViewModel>();

            var data = (from ip in context.BillPayments
                        join b in context.Bills on ip.LinkedBillId equals b.Id
                        join pm in context.PaymentMethods on ip.PaymentMethodId equals pm.PaymentMethodId
                        where b.CompanyId == companyId && b.IsDeleted == false
                        select new
                        {
                            b.VendorId,
                            b.Id,
                            ip.PaymentMethodId,
                            PaymentName = pm.Description,
                            ip.PaymentDate,
                            ReferenceNo = ip.ReferenceNumber,
                            Description = ip.DescriptionOfTransaction,
                            ip.AmountPaid,
                            b.BillNo
                        }).ToList();

            foreach (var l in data)
            {
                BillPaymentViewModel i = new BillPaymentViewModel();
                i.CustomerId = l.VendorId;
                i.BillId = l.Id;
                i.PaymentMethodId = l.PaymentMethodId;
                i.ReferenceNo = l.ReferenceNo;
                i.Description = l.Description;
                i.PaymentDate = l.PaymentDate;
                i.AmountPaid = l.AmountPaid;
                i.BillNo = l.BillNo;
                i.PaymentName = l.PaymentName;
                billPayment.Add(i);
            }

            return billPayment;
        }
    }
}
