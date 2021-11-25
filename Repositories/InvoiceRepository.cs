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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Invoice;
        public InvoiceRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        public bool Create(Invoice model, int companyId)
        {
            bool result = false;
            Guid qId = Guid.NewGuid();

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var inv = new Invoice
                    {
                        InvoiceId = qId,
                        CustomerId = model.CustomerId,
                        InvoiceNo = model.InvoiceNo,
                        CustomerPONo = model.CustomerPONo,
                        IssueDate = model.IssueDate,
                        DueDate = model.DueDate,
                        Status = "Pending",
                        CustomerNotes = model.CustomerNotes,
                        CustomerAddressId = model.CustomerAddressId,
                        SubTotal = model.SubTotal,
                        Tax = model.Tax,
                        Total = model.Total,
                        BalanceDue = model.Total,
                        CompanyId = companyId,
                        LinkedQuoteId = model.LinkedQuoteId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now,
                        PaymentTerm = model.PaymentTerm,
                        PaymentTermValue = model.PaymentTermValue,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue
                    };
                    context.Invoices.Add(inv);

                    foreach (var detail in model.invoiceDetails)
                    {
                        var invDetail = new InvoiceDetails
                        {
                            InvoiceId = qId,
                            ProductAndServiceId = detail.ProductAndServiceId,
                            Qty = detail.Qty,
                            UnitPrice = detail.UnitPrice,
                            DiscountPercent = detail.DiscountPercent,
                            Total = detail.Total,
                            DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100,
                            UOM = detail.UOM,
                            Description = detail.Description
                        };
                        context.InvoiceDetails.Add(invDetail);
                    }
                    if (model.LinkedQuoteId != new Guid() && model.LinkedQuoteId != null)
                    {
                        var quote = context.Quotes.Where(q => q.QuoteId == model.LinkedQuoteId).FirstOrDefault();
                        quote.Status = "Invoiced";
                        quote.UpdatedBy = userName;
                        quote.UpdatedDate = DateTime.Now;
                        context.Update(quote);
                    }
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = qId,
                        ModuleDescription = model.InvoiceNo,
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

        public bool Delete(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var inv = context.Invoices.Where(q => q.InvoiceId == id).FirstOrDefault();
                    inv.IsDeleted = true;
                    inv.UpdatedBy = userName;
                    inv.UpdatedDate = DateTime.Now;
                    context.Update(inv);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = inv.InvoiceNo,
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

        public CustomerAddress getCustomerAddress(int id)
        {
            CustomerAddress address = context.CustomerAddresses.SingleOrDefault(
                a => a.CustomerAddressId == id
            );
            return address;
        }

        public Invoice GetInvoice(Guid id)
        {
            Invoice inv = context.Invoices.Find(id);
            return inv;
        }

        public IEnumerable<Invoice> GetInvoiceByCompanyId(int id)
        {
            IEnumerable<Invoice> inv = context.Invoices.Where(
                q => q.CompanyId == id && q.IsDeleted == false
            );
            return inv;
        }

        public IEnumerable<InvoiceDetails> GetInvoiceDetails(Guid id)
        {
            IEnumerable<InvoiceDetails> inv = context.InvoiceDetails.Where(q => q.InvoiceId == id);
            return inv;
        }

        public bool Update(Invoice model, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var inv = context.Invoices.Where(q => q.InvoiceId == model.InvoiceId).FirstOrDefault();
                    inv.CustomerId = model.CustomerId;
                    inv.InvoiceNo = model.InvoiceNo;
                    inv.CustomerPONo = model.CustomerPONo;
                    inv.IssueDate = model.IssueDate;
                    inv.DueDate = model.DueDate;
                    //inv.Status = model.Status == null ? inv.Status : model.Status == "Closed" ? "Closed" : "Open";
                    inv.CustomerNotes = model.CustomerNotes;
                    inv.CustomerAddressId = model.CustomerAddressId;
                    inv.SubTotal = model.SubTotal;
                    inv.Tax = model.Tax;
                    inv.Total = model.Total;
                    inv.BalanceDue = model.Total;
                    inv.CompanyId = companyId;
                    inv.UpdatedBy = userName;
                    inv.UpdatedDate = DateTime.Now;
                    inv.PaymentTerm = model.PaymentTerm;
                    inv.PaymentTermValue = model.PaymentTermValue;
                    inv.DiscountType = model.DiscountType;
                    inv.DiscountValue = model.DiscountValue;
                    context.Update(inv);

                    var listdetail = context.InvoiceDetails.Where(qd => qd.InvoiceId == model.InvoiceId);

                    List<int> detList = new List<int>();
                    foreach (var l in listdetail)
                    {
                        detList.Add(l.InvoiceDetailsId);
                    }

                    foreach (var detail in model.invoiceDetails)
                    {
                        var invDetail = context.InvoiceDetails.Where(qd => qd.InvoiceDetailsId == detail.InvoiceDetailsId).FirstOrDefault();
                        if (invDetail != null)
                        {
                            detList.Remove(invDetail.InvoiceDetailsId);
                            invDetail.InvoiceId = detail.InvoiceId;
                            invDetail.ProductAndServiceId = detail.ProductAndServiceId;
                            invDetail.Qty = detail.Qty;
                            invDetail.UnitPrice = detail.UnitPrice;
                            invDetail.DiscountPercent = detail.DiscountPercent;
                            invDetail.Total = detail.Total;
                            invDetail.Description = detail.Description;
                            invDetail.UOM = detail.UOM;
                            invDetail.DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100;
                            context.Update(invDetail);
                        }
                        else
                        {
                            var newInvoiceDetail = new InvoiceDetails
                            {
                                InvoiceId = detail.InvoiceId,
                                ProductAndServiceId = detail.ProductAndServiceId,
                                Qty = detail.Qty,
                                UnitPrice = detail.UnitPrice,
                                DiscountPercent = detail.DiscountPercent,
                                Total = detail.Total,
                                Description = detail.Description,
                                UOM = detail.UOM,
                                DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100
                            };
                            context.InvoiceDetails.Add(newInvoiceDetail);
                        }
                    }
                    foreach (int i in detList)
                    {
                        InvoiceDetails q = context.InvoiceDetails.Find(i);
                        context.InvoiceDetails.Remove(q);
                    }
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = model.InvoiceId,
                        ModuleDescription = model.InvoiceNo,
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
