using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;

namespace Anastock.Controllers
{
    public class InvoiceReceivableController : Controller
    {
        private readonly AnastockContext context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceReceivableRepository _invoiceReceivableRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;
        public InvoiceReceivableController(AnastockContext context, UserManager<ApplicationUser> userManager, IInvoiceRepository invoiceRepository, IInvoiceReceivableRepository invoiceReceivableRepository, IHubContext<SignalServer> signalContext)
        {
            this._userManager = userManager;
            this.context = context;
            this._invoiceRepository = invoiceRepository;
            this._invoiceReceivableRepository = invoiceReceivableRepository;
            this._signalContext = signalContext;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Sales";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                var model = _invoiceReceivableRepository.GetInvoiceReceivables(companyId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public IActionResult save([FromBody] InvoiceReceivableViewModel invReceivable)
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
                bool result = _invoiceReceivableRepository.Create(invReceivable, companyId); ;
                if (result)
                {
                    message = "Invoice payment has been created successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
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
                    ViewBag.ListofPayment = bindPayment(companyId);
                    ViewBag.NewId = id;
                    return View();
                }
                else
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofPayment = bindPayment(companyId);
                    ViewBag.NewId = id;
                    var invoice = _invoiceRepository.GetInvoice(id);
                    if (invoice != null)
                    {
                        ViewBag.ListofCustomer = new SelectList(bindCustomer(companyId), "CustomerId", "CustomerName", invoice.CustomerId);
                        ViewBag.ListofInvoice = new SelectList(bindInvoices(invoice.CustomerId, companyId), "InvoiceId", "InvoiceNo", invoice.InvoiceId);
                        ViewBag.BalanceDue = invoice.BalanceDue.ToString("0.00");
                        return View();
                    }
                    return RedirectToAction("Index", "InvoiceReceivable");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        private List<Customer> bindCustomer(int companyId)
        {
            List<Customer> customerList = new List<Customer>();
            customerList = (from c in context.Customers
                            where c.CompanyId == companyId && c.IsDeleted == false
                            select c).ToList();
            customerList.Insert(0, new Customer { CustomerId = new Guid("00000000-0000-0000-0000-000000000000"), CustomerName = "Select Customer" });
            return customerList;
        }

        private List<PaymentMethod> bindPayment(int companyId)
        {
            List<PaymentMethod> payMethod = new List<PaymentMethod>();
            payMethod = (from c in context.PaymentMethods
                         where c.CompanyId == companyId && c.IsDeleted == false
                         select c).ToList();
            payMethod.Insert(0, new PaymentMethod { PaymentMethodId = new Guid("00000000-0000-0000-0000-000000000000"), Description = "Select Payment" });
            return payMethod;
        }

        public List<Invoice> bindInvoices(Guid customerId, int companyId)
        {
            List<Invoice> invoice = new List<Invoice>();
            invoice = (from c in context.Invoices
                       where c.CompanyId == companyId && c.CustomerId == customerId && c.IsDeleted == false && (c.Status == "Pending" || c.Status == "Partial")
                       select c).ToList();
            invoice.Insert(0, new Invoice { InvoiceId = new Guid("00000000-0000-0000-0000-000000000000"), InvoiceNo = "Select Invoice" });
            return invoice;
        }

        public ActionResult bindInvoice(Guid customerId)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            List<Invoice> invoice = new List<Invoice>();
            invoice = (from c in context.Invoices
                       where c.CompanyId == companyId && c.CustomerId == customerId && c.IsDeleted == false && c.Status == "Pending"
                       select c).ToList();
            invoice.Insert(0, new Invoice { InvoiceId = new Guid("00000000-0000-0000-0000-000000000000"), InvoiceNo = "Select Invoice" });
            return Ok(invoice);
        }

        public ActionResult bindBalance(Guid invoiceId)
        {
            var balance = context.Invoices.Where(i => i.InvoiceId == invoiceId).Select(i => i.BalanceDue).Single();
            return Ok(balance);
        }
    }
}
