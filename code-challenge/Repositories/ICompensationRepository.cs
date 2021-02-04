using System;
using System.Threading.Tasks;
using challenge.Models;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Compensation GetByEmployeeId(string id);
        Task SaveAsync();
    }
}
