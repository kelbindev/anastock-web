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
    public class InvoiceReceivableRepository : IInvoiceReceivableRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.InvoiceReceivable;
        public InvoiceReceivableRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        public List<InvoiceReceivableViewModel> GetInvoiceReceivables(int companyId)
        {
            List<InvoiceReceivableViewModel> invPayment = new List<InvoiceReceivableViewModel>();
            //var data = context.Invoices.Where(i => i.CompanyId == companyId).Join(context.InvoiceReceivables,
            //    i => i.InvoiceId,
            //    p => p.LinkedInvoiceId, (i, p) => new
            //    {
            //        i.CustomerId,
            //        i.InvoiceId,
            //        p.PaymentDate,
            //        p.PaymentMethodId,
            //        ReferenceNo = p.ReferenceNumber,
            //        Description = p.DescriptionOfTransaction,
            //        p.AmountReceived,
            //        i.InvoiceNo
            //    }).Join(context.PaymentMethods, x => x.PaymentMethodId, pm => pm.PaymentMethodId, (x, pm) => new
            //    {
            //        x.InvoiceId,
            //        x.InvoiceNo,
            //        x.PaymentMethodId,
            //        x.ReferenceNo,
            //        x.AmountReceived,
            //        x.CustomerId,
            //        x.Description,
            //        x.PaymentDate,
            //        PaymentName = pm.Description
            //    }).ToList();
            var data = (from ip in context.InvoiceReceivables
                              join i in context.Invoices on ip.LinkedInvoiceId equals i.InvoiceId
                              join pm in context.PaymentMethods on ip.PaymentMethodId equals pm.PaymentMethodId
                              where i.CompanyId == companyId && i.IsDeleted == false
                              select new
                              {
                                  i.CustomerId,
                                  i.InvoiceId,
                                  ip.PaymentMethodId,
                                  PaymentName = pm.Description,
                                  ip.PaymentDate,
                                  ReferenceNo = ip.ReferenceNumber,
                                  Description = ip.DescriptionOfTransaction,
                                  ip.AmountReceived,
                                  i.InvoiceNo
                              }).ToList();

            foreach (var l in data)
            {
                InvoiceReceivableViewModel i = new InvoiceReceivableViewModel();
                i.CustomerId = l.CustomerId;
                i.InvoiceId = l.InvoiceId;
                i.PaymentMethodId = l.PaymentMethodId;
                i.ReferenceNo = l.ReferenceNo;
                i.Description = l.Description;
                i.PaymentDate = l.PaymentDate;
                i.AmountReceived = l.AmountReceived;
                i.InvoiceNo = l.InvoiceNo;
                i.PaymentName = l.PaymentName;
                invPayment.Add(i);
            }

            return invPayment;
        }
        public bool Create(InvoiceReceivableViewModel model, int companyId)
        {
            bool result = false;
            Guid qId = Guid.NewGuid();


            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                var invoice = context.Invoices.Where(i => i.InvoiceId == model.InvoiceId).FirstOrDefault();
                //var invoiceAmount = context.Invoices.Where(i => i.InvoiceId == model.InvoiceId).Select(i => new { i.AmountPaid, i.BalanceDue }).FirstOrDefault();
                //var newBalanceDue = invoiceAmount.BalanceDue - model.AmountReceived;
                //var newAmountPaid = invoiceAmount.AmountPaid + model.AmountReceived;
                var newBalanceDue = invoice.BalanceDue - model.AmountReceived;
                var newAmountPaid = invoice.AmountPaid + model.AmountReceived;
                string status = (newBalanceDue <= 0) ? "Paid" : "Partial";
                try
                {
                    var inv = new InvoiceReceivable
                    {
                        PaymentDate = model.PaymentDate,
                        ReferenceNumber = model.ReferenceNo,
                        DescriptionOfTransaction = model.Description,
                        AmountReceived = model.AmountReceived,
                        PaymentMethodId = model.PaymentMethodId,
                        LinkedInvoiceId = model.InvoiceId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now
                    };
                    context.InvoiceReceivables.Add(inv);

                    invoice.BalanceDue = newBalanceDue;
                    context.Entry(invoice).Property("BalanceDue").IsModified = true;
                    invoice.AmountPaid = newAmountPaid;
                    context.Entry(invoice).Property("AmountPaid").IsModified = true;
                    invoice.Status = status;
                    context.Entry(invoice).Property("Status").IsModified = true;

                    //Update Product Balance
                    if (status == "Paid")
                    {
                        List<InvoiceDetails> id = context.InvoiceDetails.Where(b => b.InvoiceId == inv.LinkedInvoiceId).ToList();

                        foreach (var det in id)
                        {
                            ProductAndService p = context.ProductAndService.Where(x => x.Id == det.ProductAndServiceId).FirstOrDefault();
                            if (p.CategoryId == 1)
                            {
                                ProductBalance pb = context.productBalances.Where(x => x.ProductId == det.ProductAndServiceId).FirstOrDefault();
                                if (pb != null)
                                {
                                    decimal qty = det.Qty;

                                    if (p.SellQty > 1)
                                    {
                                        qty = qty * p.SellQty.Value;
                                    }

                                    pb.Balance -= qty;
                                    context.Update(pb);

                                    //Add to product balance details
                                    ProductBalanceDetails pbd = new ProductBalanceDetails
                                    {
                                        ProductId = pb.ProductId,
                                        Qty = qty,
                                        CreatedDate = DateTime.Now,
                                        Description =
                                        String.Format(" -{0}/{2} from Invoice {1}",
                                        qty.ToString("N0")+" " + p.UOM, 
                                        invoice.InvoiceNo,
                                        det.Qty.ToString("N0")+" " + det.UOM),
                                        LinkedInvoiceId = invoice.InvoiceId
                                    };
                                    context.ProductBalanceDetails.Add(pbd);

                                }
                            }
                        }
                    }

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = invoice.InvoiceId,
                        ModuleDescription = invoice.InvoiceNo,
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
    }
}
