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
    public class QuoteRepository : IQuoteRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Quote;

        public QuoteRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }

        public IEnumerable<Quote> GetAllQuotes()
        {
            return context.Quotes;
        }

        public Quote GetQuote(Guid id)
        {
            Quote quote = context.Quotes.Find(id);
            return quote;
        }

        public IEnumerable<QuoteDetails> GetQuoteDetails(Guid id)
        {
            IEnumerable<QuoteDetails> quote = context.QuoteDetails.Where(q => q.QuoteId == id);
            return quote;
        }
        
        public CustomerAddress getCustomerAddress(int id)
        {
            CustomerAddress address = context.CustomerAddresses.SingleOrDefault(
                a => a.CustomerAddressId == id
            );
            return address;
        }

        public IEnumerable<Quote> GetQuotesByCompanyId(int id)
        {
            IEnumerable<Quote> quote = context.Quotes.Where(
                q => q.CompanyId == id && q.IsDeleted == false
            );
            return quote;
        }

        public IEnumerable<Quote> GetWonQuotesByCompanyId(int id)
        {
            IEnumerable<Quote> quote = context.Quotes.Where(
                q => q.CompanyId == id && q.IsDeleted == false && q.Status == "Won"
            );
            return quote;
        }

        public bool Create(Quote model, int companyId)
        {
            bool result = false;
            Guid qId = Guid.NewGuid();


            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var quote = new Quote
                    {
                        QuoteId = qId,
                        CustomerId = model.CustomerId,
                        QuoteNo = model.QuoteNo,
                        CustomerPONo = model.CustomerPONo,
                        IssueDate = model.IssueDate,
                        ExpiryDate = model.ExpiryDate,
                        Status = model.Status,
                        CustomerNotes = model.CustomerNotes,
                        CustomerAddressId = model.CustomerAddressId,
                        SubTotal = model.SubTotal,
                        Tax = model.Tax,
                        Total = model.Total,
                        CompanyId = companyId,
                        LinkedProjectId = model.LinkedProjectId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue
                    };
                    context.Quotes.Add(quote);

                    foreach (var detail in model.QuoteDetails)
                    {
                        var quoteDetail = new QuoteDetails
                        {
                            QuoteId = qId,
                            ProductAndServiceId = detail.ProductAndServiceId,
                            Qty = detail.Qty,
                            UnitPrice = detail.UnitPrice,
                            DiscountPercent = detail.DiscountPercent,
                            Total = detail.Total,
                            DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100,
                            UOM = detail.UOM,
                            Description = detail.Description
                        };
                        context.QuoteDetails.Add(quoteDetail);
                    }

                    var project = context.Projects.Where(p => p.ProjectId == model.LinkedProjectId).FirstOrDefault();
                    if (project != null) {
                        project.Status = "In Use";
                        project.InUse = true;
                        project.QuoteNo = model.QuoteNo;
                        context.Update(project);
                    }

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = qId,
                        ModuleDescription = model.QuoteNo,
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

        public bool Update(Quote model, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                Guid oldprojectid = new Guid();
                try
                {
                    var quote = context.Quotes.Where(q => q.QuoteId == model.QuoteId).FirstOrDefault();
                    oldprojectid = quote.LinkedProjectId == null ? new Guid() : (Guid)quote.LinkedProjectId;
                    quote.CustomerId = model.CustomerId;
                    quote.QuoteNo = model.QuoteNo;
                    quote.CustomerPONo = model.CustomerPONo;
                    quote.IssueDate = model.IssueDate;
                    quote.ExpiryDate = model.ExpiryDate;
                    quote.Status = model.Status;
                    quote.CustomerNotes = model.CustomerNotes;
                    quote.CustomerAddressId = model.CustomerAddressId;
                    quote.SubTotal = model.SubTotal;
                    quote.Tax = model.Tax;
                    quote.Total = model.Total;
                    quote.CompanyId = companyId;
                    quote.LinkedProjectId = model.LinkedProjectId;
                    quote.UpdatedBy = userName;
                    quote.DiscountType = model.DiscountType;
                    quote.DiscountValue = model.DiscountValue;
                    context.Update(quote);

                    var listdetail = context.QuoteDetails.Where(qd => qd.QuoteId == model.QuoteId);

                    List<int> detList = new List<int>();
                    foreach (var l in listdetail)
                    {
                        detList.Add(l.QuoteDetailsId);
                    }

                    foreach (var detail in model.QuoteDetails)
                    {
                        var quoteDetail = context.QuoteDetails.Where(qd => qd.QuoteDetailsId == detail.QuoteDetailsId).FirstOrDefault();
                        if (quoteDetail != null)
                        {
                            detList.Remove(quoteDetail.QuoteDetailsId);
                            quoteDetail.QuoteId = detail.QuoteId;
                            quoteDetail.ProductAndServiceId = detail.ProductAndServiceId;
                            quoteDetail.Qty = detail.Qty;
                            quoteDetail.UnitPrice = detail.UnitPrice;
                            quoteDetail.DiscountPercent = detail.DiscountPercent;
                            quoteDetail.Total = detail.Total;
                            quoteDetail.Description = detail.Description;
                            quoteDetail.UOM = detail.UOM;
                            quoteDetail.DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100;
                            context.Update(quoteDetail);
                        }
                        else
                        {
                            var newQuoteDetail = new QuoteDetails
                            {
                                QuoteId = detail.QuoteId,
                                ProductAndServiceId = detail.ProductAndServiceId,
                                Qty = detail.Qty,
                                UnitPrice = detail.UnitPrice,
                                DiscountPercent = detail.DiscountPercent,
                                Total = detail.Total,
                                Description = detail.Description,
                                UOM = detail.UOM,
                                DiscountTotal = (detail.Qty * detail.UnitPrice * detail.DiscountPercent) / 100
                            };
                            context.QuoteDetails.Add(newQuoteDetail);
                        }
                    }
                    foreach (int i in detList)
                    {
                        QuoteDetails q = context.QuoteDetails.Find(i);
                        context.QuoteDetails.Remove(q);
                    
                    }

                    if (model.LinkedProjectId != null)
                    {
                        var project2 = context.Projects.Where(p => p.ProjectId == model.LinkedProjectId).SingleOrDefault();
                        project2.InUse = true;
                        project2.QuoteNo = model.QuoteNo;
                        project2.Status = "In Use";
                        context.Update(project2);
                    }

                    if (oldprojectid != model.LinkedProjectId && oldprojectid != new Guid())
                    {
                        var project1 = context.Projects.Where(p => p.ProjectId == oldprojectid).SingleOrDefault();
                        project1.InUse = false;
                        project1.QuoteNo = null;
                        project1.Status = "New";
                        context.Update(project1);
                    }

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = model.QuoteId,
                        ModuleDescription = model.QuoteNo,
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

        public bool Delete(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var quote = context.Quotes.Where(q => q.QuoteId == id).FirstOrDefault();
                    quote.IsDeleted = true;
                    quote.UpdatedBy = userName;
                    context.Update(quote);

                    if (quote.Status == "New")
                    {
                        var project = context.Projects.Where(p => p.ProjectId == quote.LinkedProjectId).FirstOrDefault();
                        project.InUse = false;
                        project.QuoteNo = null;
                        project.Status = "New";
                        quote.LinkedProjectId = null;
                    }

                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = quote.QuoteNo,
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

        public bool Convert(Guid id, int companyId)
        {
            bool result = false;

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var quote = context.Quotes.Where(q => q.QuoteId == id).FirstOrDefault();
                    IEnumerable<QuoteDetails> quotedetails = context.QuoteDetails.Where(qd => qd.QuoteId == id);
                    Guid invId = Guid.NewGuid();
                    Invoice inv = new Invoice();
                    inv.InvoiceId = invId;
                    inv.InvoiceNo = quote.QuoteNo;
                    inv.CustomerPONo = quote.CustomerPONo;
                    inv.IssueDate = quote.IssueDate;
                    inv.ExpiryDate = quote.ExpiryDate;
                    inv.Status = "Open";
                    inv.TaxInclusive = quote.TaxInclusive;
                    inv.CustomerNotes = quote.CustomerNotes;
                    inv.SubTotal = quote.SubTotal;
                    inv.Tax = quote.Tax;
                    inv.DiscountType = quote.DiscountType;
                    inv.DiscountValue = quote.DiscountValue;
                    inv.Total = quote.Total;
                    inv.CreditTerm = quote.CreditTerm;
                    inv.ShippingTerm = quote.ShippingTerm;
                    inv.DeliveryTerm = quote.DeliveryTerm;
                    inv.PaymentTerm = quote.PaymentTerm;
                    inv.LinkedQuoteId = quote.QuoteId;
                    inv.CustomerId = quote.CustomerId;
                    inv.CustomerAddressId = quote.CustomerAddressId;
                    inv.CompanyId = companyId;
                    context.Invoices.Add(inv);

                    foreach (var qd in quotedetails)
                    {
                        InvoiceDetails invdet = new InvoiceDetails();
                        invdet.UOM = qd.UOM;
                        invdet.Qty = qd.Qty;
                        invdet.UnitPrice = qd.UnitPrice;
                        invdet.DiscountPercent = qd.DiscountPercent;
                        invdet.DiscountTotal = qd.DiscountTotal;
                        invdet.Total = qd.Total;
                        invdet.ProductAndServiceId = qd.ProductAndServiceId;
                        invdet.InvoiceId = invId;
                        context.InvoiceDetails.Add(invdet);
                    }
                    context.SaveChanges();
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
