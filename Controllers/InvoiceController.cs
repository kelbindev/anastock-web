using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Anastock.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly AnastockContext context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        public InvoiceController(AnastockContext context, IInvoiceRepository invoiceRepository, IQuoteRepository quoteRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext, IHostingEnvironment hostingEnvironment)
        {
            this._invoiceRepository = invoiceRepository;
            this._quoteRepository = quoteRepository;
            this._userManager = userManager;
            this.context = context;
            this._signalContext = signalContext;
            this._hostingEnvironment = hostingEnvironment;
            //this.context = context;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Sales";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                var model = _invoiceRepository.GetInvoiceByCompanyId(companyId);
                ViewBag.WonQuoteList = _quoteRepository.GetWonQuotesByCompanyId(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult New(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofCustomer = bindCustomer(companyId);
                    ViewBag.GST = getGST(companyId);
                    ViewBag.NewId = id;
                    ViewBag.AutoNumber = AutoNumber(companyId);
                    ViewBag.ProjectTitle = "-";
                    ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                    return View();
                }
                else
                {
                    List<ProductAndService> product = new List<ProductAndService>();
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    string projectTitle = "-";
                    ViewBag.ListofCustomer = bindCustomer(companyId);
                    ViewBag.GST = getGST(companyId);
                    ViewBag.NewId = id;
                    string autoNumber = AutoNumber(companyId);
                    ViewBag.AutoNumber = autoNumber;
                    product = (from p in context.ProductAndService
                               where p.IsDeleted == false && p.isActive == true && p.CompanyId == companyId
                               select p).ToList();
                    ViewBag.ProductList = product;
                    Invoice invInfo;
                    var quote = _quoteRepository.GetQuote(id);
                    var quoteDetails = _quoteRepository.GetQuoteDetails(id);
                    var project = context.Projects.Where(p => p.ProjectId == quote.LinkedProjectId).SingleOrDefault();
                    if(project != null)
                    {
                        projectTitle = project.Title;
                    }
                    ViewBag.ProjectTitle = projectTitle;
                    if (quote != null)
                    {
                        var address = _invoiceRepository.getCustomerAddress(quote.CustomerAddressId);
                        List<InvoiceDetails> idl = new List<InvoiceDetails>();
                        foreach (var qd in quoteDetails)
                        {
                            idl.Add(new InvoiceDetails
                            {
                                Qty = qd.Qty,
                                UnitPrice = qd.UnitPrice,
                                DiscountPercent = qd.DiscountPercent,
                                Total = qd.Total,
                                ProductAndServiceId = qd.ProductAndServiceId,
                                Description = qd.Description,
                                UOM = qd.UOM,
                            });
                        }

                        CustomerAddress ca = address;

                        invInfo = new Invoice
                        {
                            LinkedQuoteId = quote.QuoteId,
                            CustomerId = quote.CustomerId,
                            InvoiceNo = autoNumber,
                            CustomerPONo = quote.CustomerPONo,
                            IssueDate = quote.IssueDate,
                            ExpiryDate = quote.ExpiryDate,
                            DueDate = quote.ExpiryDate,
                            Status = quote.Status,
                            CustomerNotes = quote.CustomerNotes,
                            CustomerAddress = ca,
                            CustomerAddressId = quote.CustomerAddressId,
                            SubTotal = quote.SubTotal,
                            Tax = quote.Tax,
                            Total = quote.Total,
                            CompanyId = companyId,
                            invoiceDetails = idl,
                            DiscountType = quote.DiscountType,
                            DiscountValue = quote.DiscountValue
                        };

                        return View(invInfo);
                    }
                    return RedirectToAction("Index", "Invoice");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<ProductAndService> product = new List<ProductAndService>();
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    string projectTitle = "-";
                    ViewBag.ListofCustomer = bindCustomer(companyId);
                    ViewBag.GST = getGST(companyId);
                    Invoice invInfo;
                    var invoice = _invoiceRepository.GetInvoice(Guid.Parse(id));
                    var invoiceDetails = _invoiceRepository.GetInvoiceDetails(Guid.Parse(id));
                    var address = _invoiceRepository.getCustomerAddress(invoice.CustomerAddressId);
                    if (invoice.LinkedQuoteId != null)
                    {
                        var quote = context.Quotes.Where(q => q.QuoteId == invoice.LinkedQuoteId).SingleOrDefault();
                        var project = context.Projects.Where(p => p.ProjectId == quote.LinkedProjectId).SingleOrDefault();
                        if (project != null)
                        {
                            projectTitle = project.Title;
                        }
                    }
                    ViewBag.ProjectTitle = projectTitle;
                    product = (from p in context.ProductAndService
                               where p.IsDeleted == false && p.isActive == true && p.CompanyId == companyId
                               select p).ToList();
                    ViewBag.ProductList = product;
                    if (invoice != null)
                    {
                        List<InvoiceDetails> qdl = new List<InvoiceDetails>();
                        foreach (var qd in invoiceDetails)
                        {
                            qdl.Add(new InvoiceDetails
                            {
                                InvoiceDetailsId = qd.InvoiceDetailsId,
                                Qty = qd.Qty,
                                UnitPrice = qd.UnitPrice,
                                DiscountPercent = qd.DiscountPercent,
                                Total = qd.Total,
                                InvoiceId = qd.InvoiceId,
                                ProductAndServiceId = qd.ProductAndServiceId,
                                Description = qd.Description,
                                UOM = qd.UOM
                            });
                        }

                        CustomerAddress ca = address;

                        invInfo = new Invoice
                        {
                            InvoiceId = invoice.InvoiceId,
                            CustomerId = invoice.CustomerId,
                            InvoiceNo = invoice.InvoiceNo,
                            CustomerPONo = invoice.CustomerPONo,
                            IssueDate = invoice.IssueDate,
                            DueDate = invoice.DueDate,
                            Status = invoice.Status,
                            CustomerNotes = invoice.CustomerNotes,
                            CustomerAddress = ca,
                            CustomerAddressId = invoice.CustomerAddressId,
                            SubTotal = invoice.SubTotal,
                            Tax = invoice.Tax,
                            Total = invoice.Total,
                            CompanyId = companyId,
                            BalanceDue = invoice.BalanceDue,
                            AmountPaid = invoice.AmountPaid,
                            invoiceDetails = qdl,
                            PaymentTerm = invoice.PaymentTerm,
                            PaymentTermValue = invoice.PaymentTermValue,
                            DiscountType = invoice.DiscountType,
                            DiscountValue = invoice.DiscountValue
                        };

                        return View(invInfo);
                    }
                }

                return RedirectToAction("Index", "Invoice");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult save([FromBody] Invoice invoice)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ModelErrorCollection> error = null;
            string message = "";

            if (!ModelState.IsValid)
            {
                error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Json(new { success = false, message = String.Join(Environment.NewLine, error) });
            }
            else
            {
                bool result = _invoiceRepository.Create(invoice, companyId); ;
                if (result)
                {
                    message = "Invoice has been created successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        [HttpPost]
        public IActionResult update([FromBody] Invoice invoice)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ModelErrorCollection> error = null;
            string message = "";

            if (!ModelState.IsValid)
            {
                error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return Json(new { success = false, message = String.Join(Environment.NewLine, error) });
            }
            else
            {
                bool result = _invoiceRepository.Update(invoice, companyId); ;
                if (result)
                {
                    message = "Invoice has been updated successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        public IActionResult delete(Guid id)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string message = "";

            bool result = _invoiceRepository.Delete(id, companyId); ;
            if (result)
            {
                message = "Invoice has been deleted successfully.";
                var model = _invoiceRepository.GetInvoiceByCompanyId(companyId);
                return RedirectToAction("Index", "Invoice");
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<ProductAndService> productList = new List<ProductAndService>();
            productList = (from p in context.ProductAndService
                           where p.CompanyId == companyId && p.IsDeleted == false && p.isActive == true
                           select p).ToList();

            return Ok(productList);
        }

        private List<Customer> bindCustomer(int companyId)
        {
            List<Customer> customerList = new List<Customer>();
            customerList = (from c in context.Customers
                            where c.CompanyId == companyId && c.IsDeleted == false
                            select c).ToList();
            customerList.Insert(0, new Customer { CustomerId = new Guid("00000000-0000-0000-0000-000000000000"), CustomerName = "Select Customer" });
            customerList.Insert(customerList.Count, new Customer { CustomerId = new Guid("00000000-0000-0000-0000-000000000000"), CustomerName = "Create New Customer" });
            return customerList;
        }

        public List<CustomerAddress> bindCustomerAddressById(Guid customerId)
        {
            List<CustomerAddress> customerAddressList = new List<CustomerAddress>();
            customerAddressList = (from c in context.CustomerAddresses
                                   where c.CustomerId == customerId
                                   select c).ToList();
            return customerAddressList;
        }

        private int getGST(int companyId)
        {
            int result = 0;
            var company = context.Company.SingleOrDefault(c => c.CompanyId == companyId);
            if (company != null)
            {
                result = (int)company.GST;
            }
            return result;
        }

        public ActionResult GeneratePdf(string id)
        {
            RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
            string webRootPath = _hostingEnvironment.WebRootPath;
            Guid ii = new Guid(id);

            int invoiceDetCounts = context.InvoiceDetails.Where(q => q.InvoiceId == ii && q.DiscountPercent > 0).Count();


            Config.WebMode = true;
            WebReport webReport = new WebReport();

            if (invoiceDetCounts > 0)
            {
                webReport.Report.Load(webRootPath + "/reports/template/Invoice.frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "/reports/template/InvoiceWithoutItemDiscount.frx");
            }
            webReport.Report.Dictionary.Connections[0].ConnectionString = context.Database.GetDbConnection().ConnectionString;
            webReport.Report.StartReport += delegate (object sender, EventArgs e)
            {
                webReport.Report.SetParameterValue("ii", ii);

            };
            var invoice = context.Invoices.Where(q => q.InvoiceId == ii).SingleOrDefault();
            webReport.Report.Prepare();
            using (MemoryStream ms = new MemoryStream())
            {

                PDFSimpleExport pdfExport = new PDFSimpleExport();
                pdfExport.Export(webReport.Report, ms);
                ms.Flush();
                return File(ms.ToArray(), "application/pdf", Path.GetFileNameWithoutExtension("Invoice") + "_" + invoice.InvoiceNo + ".pdf");
            }

        }

        private String AutoNumber(int companyId)
        {
            string max = context.Invoices.Where(i => i.InvoiceNo.StartsWith("IV-") && i.InvoiceNo.Length == 10 && i.CompanyId == companyId).Select(i => i.InvoiceNo).Max();
            string anumber = "0";
            if (String.IsNullOrEmpty(max))
            {
                anumber = "IV-" + 1.ToString().PadLeft(7, '0');
            }
            else
            {
                int lastNumber = Int32.Parse(max.Split("-").Last());
                anumber = "IV-" + (lastNumber + 1).ToString().PadLeft(7, '0');
            }
            return anumber;
        }
    }
}
