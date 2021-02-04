using System;
namespace challenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        public int NumberOfReports { get; set; }

        public ReportingStructure(Employee employee, int numberOfReports)
        {
            Employee = employee;
            NumberOfReports = numberOfReports;
        }
    }
}
