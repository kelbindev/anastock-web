using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Anastock.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AnastockContext context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyRepository _companyRepository;
        public CompanyController(AnastockContext context, UserManager<ApplicationUser> userManager, ICompanyRepository companyRepository)
        {
            this._userManager = userManager;
            this.context = context;
            this._companyRepository = companyRepository;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                int companyId = users.CompanyId;
                var model = _companyRepository.GetCompany(companyId);
                
                if (model.Logo != null)
                {
                    string base64String = Convert.ToBase64String(model.Logo, 0, model.Logo.Length);
                    string logostr = "data:image/png;base64," + base64String;
                    ViewBag.CompanyLogo = logostr;
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult Save(IFormFile file, string data)
        {
            var users = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            int companyId = users.CompanyId;
            string fileName = string.Empty;
            string fileExt = string.Empty;
            byte[] logo;
            //using (var reader = new StreamReader(file.OpenReadStream()))
            //{
            //    var fileContent = reader.ReadToEnd();
            //    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            //    fileName = parsedContentDisposition.FileName;
            //}
            CompanyViewModel formData = JsonConvert.DeserializeObject<CompanyViewModel>(data);
            formData.Logo = null;
            formData.LogoExtension = null;
            formData.CompanyId = companyId;
            if (file != null && file.Length > 0)
            {
                Stream stream = file.OpenReadStream();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    logo = memoryStream.ToArray();
                }
                fileName = file.FileName;
                fileExt = file.ContentType;
                formData.Logo = logo;
                formData.LogoExtension = fileExt;
            }

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
                bool result = _companyRepository.Update(formData); ;
                if (result)
                {
                    message = "Company Information has been updated successfully.";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Error occured. Please try again later.";
                    return Json(new { success = false, message = message });
                }
            }
        }
    }
}
