using Anastock.Interfaces;
using Anastock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Anastock.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggerRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool Create(Activity model)
        {
            bool result = false;

            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var act = new Activity
                {
                    Modules = model.Modules,
                    ModuleId = model.ModuleId,
                    ModuleDescription = model.ModuleDescription,
                    ActivityType = model.ActivityType,
                    User = model.User,
                    Date = DateTime.Now,
                    CompanyId = model.CompanyId
                };
                context.Activity.Add(act);

                context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                string err = ex.Message.ToString();
                result = false;
            }
            return result;
        }

        public IEnumerable<Activity> GetActivities(int companyId, int total)
        {
            if (total == 0)
            {
                var activity = context.Activity.Where(a => a.CompanyId == companyId).OrderByDescending(a => a.ActivityId);
                return activity;
            }
            else
            {
                var activity = context.Activity.Where(a => a.CompanyId == companyId).OrderByDescending(a => a.ActivityId).Take(total);
                return activity;
            }
        }

        public bool saveError(String message)
        {
            bool result = false;

            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            try
            {
                var err = new ErrorLog
                {
                    DateOccured = DateTime.Now,
                    Message = message,
                    UserName = userName
                };
                context.ErrorLog.Add(err);

                context.SaveChanges();
                result = true;
            }
            catch (Exception exc)
            {
                string err = exc.Message.ToString();
                result = false;
            }
            return result;
        }
    }
}

