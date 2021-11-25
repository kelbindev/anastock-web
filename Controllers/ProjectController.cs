using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;

namespace Anastock.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AnastockContext context;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalServer> _signalContext;
        public ProjectController(AnastockContext context, IProjectRepository projectRepository, UserManager<ApplicationUser> userManager, IHubContext<SignalServer> signalContext)
        {
            this._userManager = userManager;
            this._projectRepository = projectRepository;
            this.context = context;
            this._signalContext = signalContext;

            //this.context = context;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ActiveMenu = "Sales";
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                var model = _projectRepository.GetProjectByCompanyId(companyId);
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
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                ViewBag.ListofCustomer = bindCustomer(companyId);
                ViewBag.AutoNumber = AutoNumber(companyId);

                return PartialView("New", new Project());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult save([FromBody] Project project)
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
                Project result = _projectRepository.Create(project, companyId); ;
                if (result.ProjectId != Guid.Empty)
                {
                    message = "Project has been created successfully.";
                    return Json(new { success = true, message = message, newProjectId = result.ProjectId, newProjectName = result.Title });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    int companyId = users.CompanyId;
                    ViewBag.ListofCustomer = bindCustomer(companyId);
                    Project proj;
                    var project = _projectRepository.GetProject(Guid.Parse(id));
                    
                    if (project != null)
                    {
                        proj = new Project
                        {
                            ProjectId = project.ProjectId,
                            ProjectNo = project.ProjectNo,
                            Title = project.Title,
                            InstallationDate = project.InstallationDate,
                            InstallationTime = project.InstallationTime,
                            HandoverDate = project.HandoverDate,
                            DismantleDate = project.DismantleDate,
                            Status = project.Status,
                            TargetSales = project.TargetSales,
                            ProjectBudget = project.ProjectBudget,
                            Remarks = project.Remarks,
                            CustomerId = project.CustomerId,
                            CompanyId = companyId,
                            InUse = project.InUse
                        };
                        return PartialView("Edit", proj);
                        //return View(proj);
                    }
                }

                return RedirectToAction("Index", "Project");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult update([FromBody] Project project)
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
                Project result = _projectRepository.Update(project, companyId); ;
                if (result.ProjectId != Guid.Empty)
                {
                    message = "Project has been updated successfully.";
                    return Json(new { success = true, message = message, newProjectId = result.ProjectId, newProjectName = result.Title });
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

            bool result = _projectRepository.Delete(id, companyId); ;
            if (result)
            {
                message = "Project has been deleted successfully.";
                var model = _projectRepository.GetProjectByCompanyId(companyId);
                return RedirectToAction("Index", "Project");
            }
            else
            {
                message = "Error occured. Please try again later.";
                return Json(new { success = false, message = message });
            }
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

        private String AutoNumber(int companyId)
        {
            string max = context.Projects.Where(p => p.ProjectNo.StartsWith("P-") && p.ProjectNo.Length == 9 && p.CompanyId == companyId).Select(p => p.ProjectNo).Max();
            string anumber = "0";
            if (String.IsNullOrEmpty(max))
            {
                anumber = "P-" + 1.ToString().PadLeft(7, '0');
            }
            else
            {
                int lastNumber = Int32.Parse(max.Split("-").Last());
                anumber = "P-" + (lastNumber + 1).ToString().PadLeft(7, '0');
            }
            return anumber;
        }
    }
}
