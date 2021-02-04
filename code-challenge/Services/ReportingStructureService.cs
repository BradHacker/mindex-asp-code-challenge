using System;
using challenge.Models;
using challenge.Repositories;
using Microsoft.Extensions.Logging;

namespace challenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        private int GetDirectReportsRec(Employee employee)
        {
            _logger.LogDebug(employee.EmployeeId);

            if (employee.DirectReports == null || employee.DirectReports.Count == 0)
            {
                return 1;
            }
            else
            {
                int numberDirectReports = 1;
                foreach (Employee e in employee.DirectReports)
                {
                    Employee directReport = _employeeRepository.GetById(e.EmployeeId);
                    numberDirectReports += GetDirectReportsRec(directReport);
                }
                return numberDirectReports;
            }
        }

        public ReportingStructure GetNumberDirectReports(String id)
        {
            Employee employee = _employeeRepository.GetById(id);
            // Subtract 1 from the number so we don't count the initial employee
            return new ReportingStructure(employee, GetDirectReportsRec(employee) - 1);
        }
    }
}
