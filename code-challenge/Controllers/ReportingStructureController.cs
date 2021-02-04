using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
    [Route("api/reportingStructure")]
    public class ReportingStructureController: Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IReportingStructureService _reportingStructureService;

        public ReportingStructureController(ILogger<ReportingStructureController> logger, IReportingStructureService reportingStructureService, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _reportingStructureService = reportingStructureService;
        }

        [HttpGet("{id}")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received reporting structure request for '{id}'");

            Employee employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(_reportingStructureService.GetNumberDirectReports(id));
        }
    }
}
