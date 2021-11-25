using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anastock.Controllers
{
    public class SearchController : Controller
    {
        private readonly AnastockContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        public SearchController(AnastockContext context, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            this.context = context;
        }
        [HttpGet]
        public ActionResult Result(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    List<SearchResultViewModel> lst = UniversalSearch(companyId, id);
                    ViewBag.Str = id;
                    return View(lst);
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        private List<SearchResultViewModel> UniversalSearch(int companyId, string str)
        {
            var lst = new List<SearchResultViewModel>();

            //Project
            var project = context.Projects.Join(
                context.Customers, p => p.CustomerId, c => c.CustomerId, (p, c)
                => new
                {
                    p.ProjectId,
                    p.ProjectNo,
                    p.InstallationDate,
                    p.IsDeleted,
                    p.ProjectBudget,
                    p.Status,
                    p.CompanyId,
                    c.CustomerName
                }).Where(r => r.IsDeleted == false && r.CompanyId == companyId && (r.ProjectNo.Contains(str) || r.CustomerName.Contains(str)));
            foreach (var v in project)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.ProjectId;
                sr.ReferenceNo = v.ProjectNo;
                sr.Status = v.Status;
                sr.Date = v.InstallationDate;
                sr.Total = v.ProjectBudget;
                sr.CustomerName = v.CustomerName;
                sr.Category = "Project";
                sr.Url = "Project/Edit";
                lst.Add(sr);

            }

            //Quote
            var quote = context.Quotes.Join(context.QuoteDetails, q => q.QuoteId, qd => qd.QuoteId, (q, qd)
                => new
                {
                    q.QuoteId,
                    q.QuoteNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.CustomerId,
                    qd.ProductAndServiceId,
                    qd.Description,
                    qd.UOM,
                    qd.Qty,
                    qd.UnitPrice,
                    ItemPriceTotal = qd.Total,
                    qd.DiscountPercent
                }).Join(context.ProductAndService, q => q.ProductAndServiceId, p => p.Id, (q, p)
                => new
                {
                    q.QuoteId,
                    q.QuoteNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.CustomerId,
                    p.Name,
                    q.Description,
                    q.UOM,
                    q.Qty,
                    q.UnitPrice,
                    q.ItemPriceTotal,
                    q.DiscountPercent
                }).Join(context.Customers, q => q.CustomerId, c => c.CustomerId, (q, c)
                => new
                {
                    q.QuoteId,
                    q.QuoteNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.CustomerId,
                    q.Name,
                    q.Description,
                    q.UOM,
                    q.Qty,
                    q.UnitPrice,
                    q.ItemPriceTotal,
                    q.DiscountPercent,
                    c.CustomerName
                }).Where(r => r.IsDeleted == false && r.CompanyId == companyId && (r.QuoteNo.Contains(str) || r.CustomerName.Contains(str) || r.Name.Contains(str) || r.Description.Contains(str)));
            var quoteMaster = quote.Select(q => new
            {
                q.QuoteId,
                q.QuoteNo,
                q.IssueDate,
                q.IsDeleted,
                q.Total,
                q.Status,
                q.CompanyId,
                q.CustomerId,
                q.CustomerName
            }).Distinct().ToList();
            var quoteDetail = quote.Select(det => new {
                det.QuoteId,
                det.Name,
                det.Description,
                det.UOM,
                det.Qty,
                det.UnitPrice,
                det.ItemPriceTotal,
                det.DiscountPercent,
            }).Distinct().ToList();
            foreach (var v in quoteMaster)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.QuoteId;
                sr.ReferenceNo = v.QuoteNo;
                sr.Status = v.Status;
                sr.Date = v.IssueDate;
                sr.Total = v.Total;
                sr.CustomerName = v.CustomerName;
                sr.Category = "Quote";
                sr.Url = "Quote/Edit";
                var qdet = quoteDetail.Where(det => det.QuoteId == v.QuoteId);
                var dtl = new List<ItemDetailViewModel>();
                foreach (var d in qdet)
                {
                    if (d.Name.ToLower().Contains(str.ToLower()) || d.Description.ToLower().Contains(str.ToLower()))
                    {
                        ItemDetailViewModel itm = new ItemDetailViewModel();
                        itm.Name = d.Name;
                        itm.Price = d.UnitPrice;
                        itm.Qty = d.Qty;
                        itm.UOM = d.UOM;
                        itm.Percentage = d.DiscountPercent;
                        itm.Total = d.ItemPriceTotal;
                        itm.Description = d.Description;
                        dtl.Add(itm);
                        sr.ItemDetail = dtl;
                    }
                }

                lst.Add(sr);
            }

            //Invoice
            var invoice = context.Invoices.Join(context.InvoiceDetails, q => q.InvoiceId, qd => qd.InvoiceId, (q, qd)
                => new
                {
                    q.InvoiceId,
                    q.InvoiceNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.CustomerId,
                    qd.ProductAndServiceId,
                    qd.Description,
                    qd.UOM,
                    qd.Qty,
                    qd.UnitPrice,
                    ItemPriceTotal = qd.Total,
                    qd.DiscountPercent
                }).Join(context.ProductAndService, q => q.ProductAndServiceId, p => p.Id, (q, p)
                => new
                {
                    q.InvoiceId,
                    q.InvoiceNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.CustomerId,
                    p.Name,
                    q.Description,
                    q.UOM,
                    q.Qty,
                    q.UnitPrice,
                    q.ItemPriceTotal,
                    q.DiscountPercent
                }).Join(context.Customers, i => i.CustomerId, c => c.CustomerId, (i, c)
                => new
                {
                    i.InvoiceId,
                    i.InvoiceNo,
                    i.IssueDate,
                    i.IsDeleted,
                    i.Total,
                    i.Status,
                    i.CompanyId,
                    i.CustomerId,
                    i.Name,
                    i.Description,
                    i.UOM,
                    i.Qty,
                    i.UnitPrice,
                    i.ItemPriceTotal,
                    i.DiscountPercent,
                    c.CustomerName
                }).Where(r => r.IsDeleted == false && r.CompanyId == companyId && (r.InvoiceNo.Contains(str) || r.CustomerName.Contains(str) || r.Name.Contains(str) || r.Description.Contains(str)));
            var invoiceMaster = invoice.Select(q => new
            {
                q.InvoiceId,
                q.InvoiceNo,
                q.IssueDate,
                q.IsDeleted,
                q.Total,
                q.Status,
                q.CompanyId,
                q.CustomerId,
                q.CustomerName
            }).Distinct().ToList();
            var invoiceDetail = invoice.Select(det => new {
                det.InvoiceId,
                det.Name,
                det.Description,
                det.UOM,
                det.Qty,
                det.UnitPrice,
                det.ItemPriceTotal,
                det.DiscountPercent,
            }).Distinct().ToList();
            foreach (var v in invoiceMaster)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.InvoiceId;
                sr.ReferenceNo = v.InvoiceNo;
                sr.Status = v.Status;
                sr.Date = v.IssueDate;
                sr.Total = v.Total;
                sr.CustomerName = v.CustomerName;
                sr.Category = "Invoice";
                sr.Url = "Invoice/Edit";
                var idet = invoiceDetail.Where(det => det.InvoiceId == v.InvoiceId);
                var dtl = new List<ItemDetailViewModel>();
                foreach (var d in idet)
                {
                    if (d.Name.ToLower().Contains(str.ToLower()) || d.Description.ToLower().Contains(str.ToLower()))
                    {
                        ItemDetailViewModel itm = new ItemDetailViewModel();
                        itm.Name = d.Name;
                        itm.Price = d.UnitPrice;
                        itm.Qty = d.Qty;
                        itm.UOM = d.UOM;
                        itm.Percentage = d.DiscountPercent;
                        itm.Total = d.ItemPriceTotal;
                        itm.Description = d.Description;
                        dtl.Add(itm);
                        sr.ItemDetail = dtl;
                    }
                }

                lst.Add(sr);
            }

            //Purchase Order
            var purcharorder = context.PurchaseOrders.Join(context.PurchaseOrderDetails, q => q.Id, qd => qd.PurchaseOrderId, (q, qd)
                => new
                {
                    q.Id,
                    q.PurchaseOrderNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.VendorId,
                    qd.ProductAndServiceId,
                    qd.Description,
                    qd.UOM,
                    qd.Qty,
                    qd.UnitPrice,
                    ItemPriceTotal = qd.Total,
                    qd.DiscountPercent
                }).Join(context.ProductAndService, q => q.ProductAndServiceId, p => p.Id, (q, p)
                => new
                {
                    q.Id,
                    q.PurchaseOrderNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.VendorId,
                    p.Name,
                    q.Description,
                    q.UOM,
                    q.Qty,
                    q.UnitPrice,
                    q.ItemPriceTotal,
                    q.DiscountPercent
                }).Join(context.Vendors, p => p.VendorId, v => v.VendorId, (p, v)
                => new
                {
                    p.Id,
                    p.PurchaseOrderNo,
                    p.IssueDate,
                    p.IsDeleted,
                    p.Total,
                    p.Status,
                    p.CompanyId,
                    p.VendorId,
                    p.Name,
                    p.Description,
                    p.UOM,
                    p.Qty,
                    p.UnitPrice,
                    p.ItemPriceTotal,
                    p.DiscountPercent,
                    v.VendorName
                }).Where(r => r.IsDeleted == false && r.CompanyId == companyId && (r.PurchaseOrderNo.Contains(str) || r.VendorName.Contains(str) || r.Name.Contains(str) || r.Description.Contains(str)));

            var poMaster = purcharorder.Select(q => new
            {
                q.Id,
                q.PurchaseOrderNo,
                q.IssueDate,
                q.IsDeleted,
                q.Total,
                q.Status,
                q.CompanyId,
                q.VendorId,
                q.VendorName
            }).Distinct().ToList();
            var poDetail = purcharorder.Select(det => new {
                det.Id,
                det.Name,
                det.Description,
                det.UOM,
                det.Qty,
                det.UnitPrice,
                det.ItemPriceTotal,
                det.DiscountPercent,
            }).Distinct().ToList();
            foreach (var v in poMaster)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.Id;
                sr.ReferenceNo = v.PurchaseOrderNo;
                sr.Status = v.Status;
                sr.Date = v.IssueDate;
                sr.Total = v.Total;
                sr.VendorName = v.VendorName;
                sr.Category = "PurchaseOrder";
                sr.Url = "PurchaseOrder/Edit";
                var podet = poDetail.Where(det => det.Id == v.Id);
                var dtl = new List<ItemDetailViewModel>();
                foreach (var d in podet)
                {
                    if (d.Name.ToLower().Contains(str.ToLower()) || d.Description.ToLower().Contains(str.ToLower()))
                    {
                        ItemDetailViewModel itm = new ItemDetailViewModel();
                        itm.Name = d.Name;
                        itm.Price = d.UnitPrice;
                        itm.Qty = d.Qty;
                        itm.UOM = d.UOM;
                        itm.Percentage = d.DiscountPercent;
                        itm.Total = d.ItemPriceTotal;
                        itm.Description = d.Description;
                        dtl.Add(itm);
                        sr.ItemDetail = dtl;
                    }
                }

                lst.Add(sr);
            }

            //Bill
            var bill = context.Bills.Join(context.billDetails, q => q.Id, qd => qd.BillId, (q, qd)
                => new
                {
                    q.Id,
                    q.BillNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.VendorId,
                    qd.ProductAndServiceId,
                    qd.Description,
                    qd.UOM,
                    qd.Qty,
                    qd.UnitPrice,
                    ItemPriceTotal = qd.Total,
                    qd.DiscountPercent
                }).Join(context.ProductAndService, q => q.ProductAndServiceId, p => p.Id, (q, p)
                => new
                {
                    q.Id,
                    q.BillNo,
                    q.IssueDate,
                    q.IsDeleted,
                    q.Total,
                    q.Status,
                    q.CompanyId,
                    q.VendorId,
                    p.Name,
                    q.Description,
                    q.UOM,
                    q.Qty,
                    q.UnitPrice,
                    q.ItemPriceTotal,
                    q.DiscountPercent
                }).Join(context.Vendors, b => b.VendorId, v => v.VendorId, (b, v)
                => new
                {
                    b.Id,
                    b.BillNo,
                    b.IssueDate,
                    b.IsDeleted,
                    b.Total,
                    b.Status,
                    b.CompanyId,
                    b.VendorId,
                    b.Name,
                    b.Description,
                    b.UOM,
                    b.Qty,
                    b.UnitPrice,
                    b.ItemPriceTotal,
                    b.DiscountPercent,
                    v.VendorName
                }).Where(r => r.IsDeleted == false && r.CompanyId == companyId && (r.BillNo.Contains(str) || r.VendorName.Contains(str) || r.Name.Contains(str) || r.Description.Contains(str)));

            var billMaster = bill.Select(q => new
            {
                q.Id,
                q.BillNo,
                q.IssueDate,
                q.IsDeleted,
                q.Total,
                q.Status,
                q.CompanyId,
                q.VendorId,
                q.VendorName
            }).Distinct().ToList();
            var billDetail = bill.Select(det => new {
                det.Id,
                det.Name,
                det.Description,
                det.UOM,
                det.Qty,
                det.UnitPrice,
                det.ItemPriceTotal,
                det.DiscountPercent,
            }).Distinct().ToList();
            foreach (var v in billMaster)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.Id;
                sr.ReferenceNo = v.BillNo;
                sr.Status = v.Status;
                sr.Date = v.IssueDate;
                sr.Total = v.Total;
                sr.VendorName = v.VendorName;
                sr.Category = "Bill";
                sr.Url = "Bill/Edit";
                var billdet = billDetail.Where(det => det.Id == v.Id);
                var dtl = new List<ItemDetailViewModel>();
                foreach (var d in billdet)
                {
                    if (d.Name.ToLower().Contains(str.ToLower()) || d.Description.ToLower().Contains(str.ToLower()))
                    {
                        ItemDetailViewModel itm = new ItemDetailViewModel();
                        itm.Name = d.Name;
                        itm.Price = d.UnitPrice;
                        itm.Qty = d.Qty;
                        itm.UOM = d.UOM;
                        itm.Percentage = d.DiscountPercent;
                        itm.Total = d.ItemPriceTotal;
                        itm.Description = d.Description;
                        dtl.Add(itm);
                        sr.ItemDetail = dtl;
                    }
                }

                lst.Add(sr);
            }

            //Vendor
            var vendor = context.Vendors.Where(q => q.IsDeleted == false && q.CompanyId == companyId && q.VendorName.Contains(str));
            foreach (var v in vendor)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.VendorId;
                sr.Name = v.VendorName;
                sr.Email = v.VendorEmail;
                sr.Description = String.IsNullOrEmpty(v.Description) ? "-" : v.Description;
                sr.Category = "Vendor";
                sr.Url = "Vendor/Index";
                lst.Add(sr);
            }

            //Customer
            var customer = context.Customers.Where(q => q.IsDeleted == false && q.CompanyId == companyId && q.CustomerName.Contains(str));
            foreach (var v in customer)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.CustomerId;
                sr.Name = v.CustomerName;
                sr.Email = v.CustomerEmail;
                sr.Description = String.IsNullOrEmpty(v.Description) ? "-" : v.Description;
                sr.Category = "Customer";
                sr.Url = "Customer/Index";
                lst.Add(sr);
            }

            //Product/Service
            var items = context.ProductAndService.Where(q => q.IsDeleted == false && q.CompanyId == companyId && q.Name.Contains(str));
            foreach (var v in items)
            {
                SearchResultViewModel sr = new SearchResultViewModel();
                sr.Id = v.Id;
                sr.Name = v.Name;
                sr.SellPrice = v.SellPrice == null ? 0 : v.SellPrice;
                sr.PurchasePrice = v.PurchasePrice == null ? 0 : v.PurchasePrice;
                sr.SellUOM = v.SellUOM == null ? "-" : v.SellUOM;
                sr.PurchaseUOM = v.PurchaseUOM == null ? "-" : v.PurchaseUOM;
                sr.ItemType = v.CategoryId == 1 ? "Product" : "Service";
                sr.Category = "Product/Service";
                sr.Url = "ProductAndService/Index";
                lst.Add(sr);
            }

            return lst;
        }
    }
}
