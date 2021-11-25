using Anastock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anastock.Interfaces
{
    public interface ILoggerRepository
    {
        IEnumerable<Activity> GetActivities(int companyId, int total);
        bool Create(Activity model);
        bool saveError(String ex);
    }
}
