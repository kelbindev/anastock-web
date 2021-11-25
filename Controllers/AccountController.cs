using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anastock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anastock.Controllers
{
    public class AccountController : Controller
    {
        private readonly AnastockContext context;
        private readonly UserManager<ApplicationUser> um;
        private readonly RoleManager<IdentityRole> rm;
        private readonly SignInManager<ApplicationUser> sim;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, AnastockContext context)
        {
            um = userManager;
            rm = roleManager;
            sim = signInManager;
            this.context = context;
        }

        private List<CompanyViewModel> bindCompany()
        {
            List<CompanyViewModel> companyList = new List<CompanyViewModel>();
            companyList = (from product in context.Company
                           select product).ToList();
            companyList.Insert(0, new CompanyViewModel { CompanyId = 0, Name = "Select Company" });

            return companyList;
        }

        private List<RoleViewModel> bindRole()
        {
            List<RoleViewModel> roleList = new List<RoleViewModel>();
            roleList = (from role in context.Roles
                           select role).ToList();
            roleList.Insert(0, new RoleViewModel { id = "Select Role", Name = "Select Role" });

            return roleList;
        }

        public IActionResult Register()
        {
            ViewBag.ListofCompanies = bindCompany();
            ViewBag.ListofRoles = bindRole();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    CompanyId = model.CompanyId,
                    UserName = model.Email,
                    Email = model.Email,
                };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var result = await um.CreateAsync(user, model.Password);
                        string roleName = (from a in context.Roles where a.id == model.RoleId select a.Name).FirstOrDefault();
                        if (result.Succeeded)
                        {
                            await um.AddToRoleAsync(user, roleName);
                            transaction.Commit();
                            await sim.SignInAsync(user, isPersistent: false);

                            return RedirectToAction("Index", "Dashboard");
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                    }
                    
                }
                    

            }
            ViewBag.ListofCompanies = bindCompany();
            ViewBag.ListofRoles = bindRole();
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await sim.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await sim.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
