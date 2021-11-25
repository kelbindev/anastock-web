using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Anastock.Controllers
{
    public class QuoteController : Controller
    {
        private readonly AnastockContext context;
        private readonly IQuoteRepository _quoteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        public QuoteController(AnastockContext context, IQuoteRepository quoteRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext, IHostingEnvironment hostingEnvironment)
        {
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
                var model = _quoteRepository.GetQuotesByCompanyId(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult New()
        {
            if (User.Identity.IsAuthenticated)
            {
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                ViewBag.ListofProjects = bindProjects(companyId);
                ViewBag.ListofCustomer = bindCustomer(companyId);
                ViewBag.AutoNumber = AutoNumber(companyId);
                ViewBag.GST = getGST(companyId);
                ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
                return View();
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
                    ViewBag.ListofCustomer = bindCustomer(companyId);
                    ViewBag.GST = getGST(companyId);
                    Quote quoteInfo;
                    var quote = _quoteRepository.GetQuote(Guid.Parse(id));
                    var quoteDetails = _quoteRepository.GetQuoteDetails(Guid.Parse(id));
                    var address = _quoteRepository.getCustomerAddress(quote.CustomerAddressId);
                    ViewBag.ListofProjects = bindListOfProject(quote.LinkedProjectId == null ? new Guid() : (Guid)quote.LinkedProjectId, companyId, quote.CustomerId);
                    product = (from p in context.ProductAndService
                               where p.IsDeleted == false && p.isActive == true && p.CompanyId == companyId
                               select p).ToList();
                    ViewBag.ProductList = product;
                    if (quote != null)
                    {
                        List<QuoteDetails> qdl = new List<QuoteDetails>();
                        foreach (var qd in quoteDetails)
                        {
                            qdl.Add(new QuoteDetails
                            {
                                QuoteDetailsId = qd.QuoteDetailsId,
                                Qty = qd.Qty,
                                UnitPrice = qd.UnitPrice,
                                DiscountPercent = qd.DiscountPercent,
                                Total = qd.Total,
                                QuoteId = qd.QuoteId,
                                ProductAndServiceId = qd.ProductAndServiceId,
                                Description = qd.Description,
                                UOM = qd.UOM
                            });
                        }

                        CustomerAddress ca = address;

                        quoteInfo = new Quote
                        {
                            QuoteId = quote.QuoteId,
                            CustomerId = quote.CustomerId,
                            QuoteNo = quote.QuoteNo,
                            CustomerPONo = quote.CustomerPONo,
                            IssueDate = quote.IssueDate,
                            ExpiryDate = quote.ExpiryDate,
                            Status = quote.Status,
                            CustomerNotes = quote.CustomerNotes,
                            CustomerAddress = ca,
                            CustomerAddressId = quote.CustomerAddressId,
                            SubTotal = quote.SubTotal,
                            Tax = quote.Tax,
                            Total = quote.Total,
                            CompanyId = companyId,
                            LinkedProjectId = quote.LinkedProjectId,
                            QuoteDetails = qdl,
                            DiscountValue = quote.DiscountValue,
                            DiscountType = quote.DiscountType
                        };

                        return View(quoteInfo);
                    }
                }

                return RedirectToAction("Index", "Quote");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            List<ProductAndService> productList = new List<ProductAndService>();
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            productList = (from product in context.ProductAndService
                           where product.CompanyId == companyId && product.IsDeleted == false && product.isActive == true
                           select product).ToList();

            return Ok(productList);
        }

        [ValidateAntiForgeryToken]
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

        [HttpPost]
        public IActionResult save([FromBody] Quote quote)
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
                bool result = _quoteRepository.Create(quote, companyId); ;
                if (result)
                {
                    message = "Quote has been created successfully.";
                    //_signalContext.Clients.All.SendAsync("refreshQuote");
                    return Json(new { success = true, message = message });
                    //return RedirectToAction("Index", "Quote");
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        [HttpPost]
        public IActionResult update([FromBody] Quote quote)
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
                bool result = _quoteRepository.Update(quote, companyId); ;
                if (result)
                {
                    message = "Quote has been updated successfully.";
                    //_signalContext.Clients.All.SendAsync("refreshQuote");
                    return Json(new { success = true, message = message });
                    //return RedirectToAction("Index", "Quote");
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

            bool result = _quoteRepository.Delete(id, companyId); ;
            if (result)
            {
                message = "Quote has been deleted successfully.";
                var model = _quoteRepository.GetQuotesByCompanyId(companyId);
                return RedirectToAction("Index", "Quote");
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
        }
        public List<CustomerAddress> bindCustomerAddressById(Guid customerId)
        {
            List<CustomerAddress> customerAddressList = new List<CustomerAddress>();
            //customerList = context.Customers.Where(x => x.CompanyId == 1).Select(x => new Customer
            //{
            //    CustomerId = x.CustomerId,
            //    CustomerName = x.CustomerName,
            //    customerAddresses = (ICollection<CustomerAddress>)x.customerAddresses.Where(c => c.CustomerId == x.CustomerId).Select(c => new CustomerAddress
            //    {
            //        BillingCountry = c.BillingCountry,
            //        CustomerId = c.CustomerId,
            //        BillingAddress = c.BillingAddress
            //    })
            //}).ToList();
            customerAddressList = (from c in context.CustomerAddresses
                                   where c.CustomerId == customerId
                                   select c).ToList();
            return customerAddressList;
        }

        public List<Project> bindProjects(int companyId)
        {
            List<Project> projects = new List<Project>();
            projects = (from c in context.Projects
                        where c.CompanyId == companyId && c.InUse == false && c.IsDeleted == false && (c.Status  == "Completed" || c.Status == "Confirmed" || c.Status == "New") 
                        select c).ToList();
            projects.Insert(0, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Select Project" });
            projects.Insert(projects.Count, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Create New Project" });
            return projects;
        }

        public List<Project> bindProjectsByCustomer(Guid customerId)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<Project> projects = new List<Project>();
            projects = (from c in context.Projects
                        where c.CompanyId == companyId && c.CustomerId == customerId && c.InUse == false && c.IsDeleted == false && (c.Status == "Completed" || c.Status == "Confirmed" || c.Status == "New")
                        select c).ToList();
            projects.Insert(0, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Select Project" });
            projects.Insert(projects.Count, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Create New Project" });
            return projects;
        }

        public List<Project> bindListOfProject(Guid projectId, int companyId, Guid customerId)
        {
            List<Project> projects = new List<Project>();
            if (projectId == new Guid())
            {
                projects = (from c in context.Projects
                            where (c.CompanyId == companyId && c.IsDeleted == false && c.InUse == false && (c.Status == "Completed" || c.Status == "Confirmed" || c.Status == "New") && c.CustomerId == customerId)
                            select c).ToList();
            }
            else
            {
                projects = (from c in context.Projects
                            where (c.CompanyId == companyId && c.IsDeleted == false && c.InUse == false && (c.Status == "Completed" || c.Status == "Confirmed" || c.Status == "New") && c.CustomerId == customerId) || c.ProjectId == projectId
                            select c).ToList();
            }
            projects.Insert(0, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Select Project" });
            projects.Insert(projects.Count, new Project { ProjectId = new Guid("00000000-0000-0000-0000-000000000000"), Title = "Create New Project" });
            return projects;
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

        [HttpGet]
        public ActionResult GeneratePdf(string id)
        {
            RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
         
            string webRootPath = _hostingEnvironment.WebRootPath;
            Guid qi = new Guid(id);

            int quoteDetCounts = context.QuoteDetails.Where(q => q.QuoteId == qi && q.DiscountPercent > 0).Count();

            Config.WebMode = true;
            WebReport webReport = new WebReport();

            if (quoteDetCounts > 0)
            {
                webReport.Report.Load(webRootPath + "/reports/template/Quote.frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "/reports/template/QuoteWithoutItemDiscount.frx");
            }
            webReport.Report.Dictionary.Connections[0].ConnectionString = context.Database.GetDbConnection().ConnectionString;
            webReport.Report.StartReport += delegate (object sender, EventArgs e)
            {
                webReport.Report.SetParameterValue("qi", qi);

            };
            var quote = context.Quotes.Where(q => q.QuoteId == qi).SingleOrDefault();
            webReport.Report.Prepare();
            using (MemoryStream ms = new MemoryStream())
            {

                PDFSimpleExport pdfExport = new PDFSimpleExport();
                pdfExport.Export(webReport.Report, ms);
                ms.Flush();
                return File(ms.ToArray(), "application/pdf", Path.GetFileNameWithoutExtension("Quote") + "_" + quote.QuoteNo + ".pdf");
            }
        }

        private String AutoNumber(int companyId)
        {
            string max = context.Quotes.Where(q => q.QuoteNo.StartsWith("Q-") && q.QuoteNo.Length == 9 && q.CompanyId == companyId).Select(q => q.QuoteNo).Max();
            string anumber = "0";
            if (String.IsNullOrEmpty(max))
            {
                anumber = "Q-" + 1.ToString().PadLeft(7, '0');
            }
            else
            {
                int lastNumber = Int32.Parse(max.Split("-").Last());
                anumber = "Q-" + (lastNumber + 1).ToString().PadLeft(7, '0');
            }
            return anumber;
        }
        [HttpGet]
        public IActionResult GetNewProjectNumber()
        {
            string anumber = "0";
            string msg = "";
            bool sucess = true;
            try
            {
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;

                string max = context.Projects.Where(p => p.ProjectNo.StartsWith("P-") && p.ProjectNo.Length == 9 && p.CompanyId == companyId).Select(p => p.ProjectNo).Max();

                if (String.IsNullOrEmpty(max))
                {
                    anumber = "P-" + 1.ToString().PadLeft(7, '0');
                }
                else
                {
                    int lastNumber = Int32.Parse(max.Split("-").Last());
                    anumber = "P-" + (lastNumber + 1).ToString().PadLeft(7, '0');
                }
            }
            catch (Exception ex)
            {
                sucess = false;
                msg = ex.ToString();
            }

            return Json(new { success= sucess, projectno = anumber, message = msg });
        }
    }

}
