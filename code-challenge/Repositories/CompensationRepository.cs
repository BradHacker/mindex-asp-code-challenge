using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly CompensationContext _compensationContext;

        public CompensationRepository(CompensationContext compensationContext)
        {
            _compensationContext = compensationContext;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _compensationContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(string id)
        {
            return _compensationContext.Compensations.Include(c => c.Employee).SingleOrDefault(c => c.Employee.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }
    }
}
