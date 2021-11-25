using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Code;
using Anastock.Interfaces;
using Anastock.Models;
using Anastock.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Anastock.Code.Common;

namespace Anastock.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AnastockContext context;
        private readonly ILoggerRepository loggerRepository;
        private readonly UserManager<ApplicationUser> userManager;
        public DashboardController(AnastockContext context, UserManager<ApplicationUser> userManager, ILoggerRepository loggerRepository)
        {
            this.context = context;
            this.userManager = userManager;
            this.loggerRepository = loggerRepository;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public List<SalesChartModel> GetSalesData()
        {
            var users = userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            DateTime dtStart = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)).AddMonths(-11);
            DateTime dtEnd = DateTime.Now;
            var dts = Enumerable.Range(0, 13).Select(a => dtStart.AddMonths(a))
               .TakeWhile(a => a <= dtEnd)
               .Select(a => String.Concat(a.ToString("MMM") + "'" + a.Year)).AsEnumerable().ToArray();
            List<SalesChartModel> lst = new List<SalesChartModel>();
            for (int i = 0; i < dts.Length; i += 1)
            {
                SalesChartModel scm = new SalesChartModel();
                scm.Period = dts[i];
                scm.Total = 0;
                scm.BalanceDue = 0;
                lst.Add(scm);
            }
                var sales = context.Invoices.Where(
                    i => (i.IssueDate >= dtStart && i.IssueDate <= dtEnd) && i.CompanyId == companyId
                ).GroupBy(
                        g => new { Mnth = g.IssueDate.Month, Yr = g.IssueDate.Year.ToString() }
                    ).Select(s => new SalesChartModel
                    {
                        Period = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(s.Key.Mnth) + "'" + s.Key.Yr,
                        Total = s.Sum(t => t.Total),
                        BalanceDue = s.Sum(b => b.BalanceDue)
                    }).ToList();
            foreach (var x in sales)
            {
                var value = lst.FirstOrDefault(d => d.Period == x.Period);
                if (value != null)
                {
                    value.Total = x.Total;
                    value.BalanceDue = x.BalanceDue;
                }
            }
            return lst;
        }

        public List<ActivityViewModel> GetActivityData()
        {
            var users = userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;

            var activities = loggerRepository.GetActivities(companyId, 5);
            List<ActivityViewModel> lst = new List<ActivityViewModel>();
            foreach (var act in activities)
            {
                string desc = string.Empty;
                string activity = act.ActivityType == ActivityType.Cancel ? "Cancelled" : Enum.GetName(typeof(ActivityType), act.ActivityType) + "d";
                if (act.Modules == Modules.Company)
                {
                    desc = "Company Information has been updated";
                }
                else if (act.Modules == Modules.InvoiceReceivable || act.Modules == Modules.BillPayment)
                {
                    string type = act.Modules == Modules.InvoiceReceivable ? "Invoice" : "Bill";
                    desc = act.Modules.GetDisplayName() + " has been " + activity + " for " + type + " " + act.ModuleDescription;
                }
                else
                {
                    desc = Enum.GetName(typeof(Modules), act.Modules) + " " + act.ModuleDescription + " has been " + activity;
                }
                ActivityViewModel a = new ActivityViewModel();
                a.Module = Enum.GetName(typeof(Modules), act.Modules);
                a.ModuleId = act.ModuleId;
                a.User = act.User;
                a.ActivityType = Enum.GetName(typeof(ActivityType), act.ActivityType);
                a.Description = desc;
                lst.Add(a);
            }
            return lst;
        }
    }
}
