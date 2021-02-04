using System;
using challenge.Models;

namespace challenge.Services
{
    public interface IReportingStructureService
    {
        ReportingStructure GetNumberDirectReports(String id);
    }
}
