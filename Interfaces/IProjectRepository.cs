using Anastock.Migrations.Anastock;
using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface IProjectRepository
    {
        Project GetProject(Guid id);
        IEnumerable<Project> GetProjectByCompanyId(int id);
        Project Create(Project model, int companyId);
        Project Update(Project model, int companyId);
        bool Delete(Guid id, int companyId);
    }
}
