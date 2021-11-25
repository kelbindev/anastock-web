using Anastock.Interfaces;
using Anastock.Migrations.Anastock;
using Anastock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Anastock.Code.Common;

namespace Anastock.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AnastockContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerRepository loggerRepository;
        private Modules module = Modules.Project;
        public ProjectRepository(AnastockContext context, IHttpContextAccessor httpContextAccessor, ILoggerRepository loggerRepository)
        {
            this.context = context;
            _httpContextAccessor = httpContextAccessor;
            this.loggerRepository = loggerRepository;
        }
        public Project Create(Project model, int companyId)
        {
            Project result = new Project();
            Guid qId = Guid.NewGuid();
            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var project = new Project
                    {
                        ProjectId = qId,
                        ProjectNo = model.ProjectNo,
                        Title = model.Title,
                        InstallationDate = model.InstallationDate,
                        InstallationTime = model.InstallationTime,
                        HandoverDate = model.HandoverDate,
                        DismantleDate = model.DismantleDate,
                        Status = model.Status,
                        TargetSales = model.TargetSales,
                        ProjectBudget = model.ProjectBudget,
                        Remarks = model.Remarks,
                        CompanyId = companyId,
                        CustomerId = model.CustomerId,
                        CreatedBy = userName,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = userName,
                        UpdatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    context.Projects.Add(project);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = qId,
                        ModuleDescription = model.ProjectNo,
                        ActivityType = ActivityType.Create,
                        User = userName,
                        CompanyId = companyId
                    };
                    loggerRepository.Create(act);


                    result = project;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    string err = ex.Message.ToString();
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
                    var project = context.Projects.Where(q => q.ProjectId == id).FirstOrDefault();
                    project.IsDeleted = true;
                    project.UpdatedBy = userName;
                    project.UpdatedDate = DateTime.Now;
                    context.Update(project);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = id,
                        ModuleDescription = project.ProjectNo,
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

        public Project GetProject(Guid id)
        {
            Project project = context.Projects.Find(id);
            return project;
        }

        public IEnumerable<Project> GetProjectByCompanyId(int id)
        {
            IEnumerable<Project> project = context.Projects.Where(
                p => p.CompanyId == id && p.IsDeleted == false
            );
            return project;
        }

        public Project Update(Project model, int companyId)
        {
            Project result = new Project();

            using (var transaction = context.Database.BeginTransaction())
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                try
                {
                    var project = context.Projects.Where(q => q.ProjectId == model.ProjectId).FirstOrDefault();
                    project.ProjectNo = model.ProjectNo;
                    project.Title = model.Title;
                    project.InstallationDate = model.InstallationDate;
                    //project.InstallationTime = model.InstallationTime;
                    project.HandoverDate = model.HandoverDate;
                    project.DismantleDate = model.DismantleDate;
                    //project.Status = model.Status;
                    //project.TargetSales = model.TargetSales;
                    project.ProjectBudget = model.ProjectBudget;
                    project.Remarks = model.Remarks;
                    project.CustomerId = model.CustomerId;
                    project.CompanyId = companyId;
                    project.UpdatedBy = userName;
                    project.UpdatedDate = DateTime.Now;
                    context.Update(project);
                    context.SaveChanges();

                    Activity act = new Activity
                    {
                        Modules = module,
                        ModuleId = model.ProjectId,
                        ModuleDescription = model.ProjectNo,
                        ActivityType = ActivityType.Update,
                        User = userName,
                        CompanyId = companyId
                    };
                    loggerRepository.Create(act);

                    result = project;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    string err = ex.Message.ToString();
                    transaction.Rollback();
                    loggerRepository.saveError(ex.ToString());
                }

            }
            return result;
        }
    }
}
