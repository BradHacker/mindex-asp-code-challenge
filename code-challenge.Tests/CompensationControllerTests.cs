using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {

            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            Compensation compensation = new Compensation()
            {
                Employee = new Employee()
                {
                    EmployeeId = employeeId,
                },
                Salary = 60000,
                EffectiveDate = new DateTime(2021, 1, 1)
            };

            string requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/compensation/", new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(expectedFirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(expectedLastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created_For_New_Employee()
        {
            // Arrange
            Employee employee = new Employee()
            {
                Department = "Development",
                FirstName = "Bradley",
                LastName = "Harker",
                Position = "ASP.NET Dev",
            };
            Compensation compensation = new Compensation()
            {
                Employee = employee,
                Salary = 100000,
                EffectiveDate = new DateTime(2021, 1, 1)
            };

            string requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/compensation/", new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(employee.Department, newCompensation.Employee.Department);
            Assert.AreEqual(employee.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(employee.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(employee.Position, newCompensation.Employee.Position);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensation_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            Compensation expectedCompensation = new Compensation()
            {
                Employee = new Employee()
                {
                    EmployeeId = employeeId,
                },
                Salary = 60000,
                EffectiveDate = new DateTime(2021, 1, 1)
            };

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Compensation compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedFirstName, compensation.Employee.FirstName);
            Assert.AreEqual(expectedLastName, compensation.Employee.LastName);
            Assert.AreEqual(expectedCompensation.Salary, compensation.Salary);
            Assert.AreEqual(expectedCompensation.EffectiveDate, compensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Invalid-ID";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
