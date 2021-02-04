# Mindex ASP.NET Code Challenge - Notes

**Note: Running the tests with `dotnet test` all test pass, however when running them in Visual Studio Community, one test seems to fail. I was unable to determine the source of this error.**

## Finding 1

There seemed to actually be an error in the existing code making the return of the `/api/employee/{id}` to not include the `directReports` field of the model. This is a breaking bug that eliminates the possibility of viewing Employee relationships designated in the database.

**Note: I was unable to figure out how to recursively do this such that all `directReports` fields at all levels were filled in. However using this data, the client application is able to traverse the tree by making multiple calls with each employee in the `directReports` field, but I am aware this is not optimal.**

### EmployeeRepository.cs (Line 32)
```C#
// ORIGINAL
return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
//NEW
return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
```

I Also added the associated tests to ensure this mistake is caught in the future.

### EmployeeControllerTests.cs (Line 91-97)
```C#
// ADDED
var expectedDirectReportFirstName = "Paul";
var expectedDirectReportLastName = "McCartney";
Assert.IsNotNull(employee.DirectReports);
Assert.AreEqual(expectedDirectReportFirstName, employee.DirectReports.ToArray()[0].FirstName);
Assert.AreEqual(expectedDirectReportLastName, employee.DirectReports.ToArray()[0].LastName);
```

### Web Result - Before
```json
{
  "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
  "firstName": "John",
  "lastName": "Lennon",
  "position": "Development Manager",
  "department": "Engineering",
  "directReports": null
}
```

### Web Result - After
```json
{
  "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
  "firstName": "John",
  "lastName": "Lennon",
  "position": "Development Manager",
  "department": "Engineering",
  "directReports": [
    {
      "employeeId": "b7839309-3348-463b-a7e3-5de1c168beb3",
      "firstName": "Paul",
      "lastName": "McCartney",
      "position": "Developer I",
      "department": "Engineering",
      "directReports": null
    },
    {
      "employeeId": "03aa1462-ffa9-4978-901b-7c001562cf6f",
      "firstName": "Ringo",
      "lastName": "Starr",
      "position": "Developer V",
      "department": "Engineering",
      "directReports": null
    }
  ]
}
```

## Task 1

I was tasked with being able to map the amount of employees that have to report to a single employee. This task was not that hard to complete, as I just went the route of recursive tree traversal to count all the employees under any given employee. Relevant tests were added in `ReportingStructureControllerTests.cs`.

### [GET] Getting the Reporting Structure

**Request:**
```
http://localhost:8080/api/reportingStructure/16a596ae-edd3-4847-99fe-c4518e82c86f
```

**Response:**
```json
{
    "employee": {
        "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
        "firstName": "John",
        "lastName": "Lennon",
        "position": "Development Manager",
        "department": "Engineering",
        "directReports": [
            {
                "employeeId": "b7839309-3348-463b-a7e3-5de1c168beb3",
                "firstName": "Paul",
                "lastName": "McCartney",
                "position": "Developer I",
                "department": "Engineering",
                "directReports": []
            },
            {
                "employeeId": "03aa1462-ffa9-4978-901b-7c001562cf6f",
                "firstName": "Ringo",
                "lastName": "Starr",
                "position": "Developer V",
                "department": "Engineering",
                "directReports": [
                    {
                        "employeeId": "62c1084e-6e34-4630-93fd-9153afb65309",
                        "firstName": "Pete",
                        "lastName": "Best",
                        "position": "Developer II",
                        "department": "Engineering",
                        "directReports": []
                    },
                    {
                        "employeeId": "c0c2293d-16bd-4603-8e08-638a9d18b22c",
                        "firstName": "George",
                        "lastName": "Harrison",
                        "position": "Developer III",
                        "department": "Engineering",
                        "directReports": []
                    }
                ]
            }
        ]
    },
    "numberOfReports": 4
}
```

## Task 2

Creating the `Compensation` model and its associated service, repository, and controller was done to model the code style of the existing structures.

### [POST] Create Compensation - http://localhost:8080/api/compensation/

**Request Body (`application/json`):**
```json
{
    "employee": {
        "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f"
    },
    "salary": "66000",
    "effectiveDate": "2021-02-04T01:59:15.671Z"
}
```

**Response:**
```json
{
    "compensationId": "f4acf94a-4bf5-4e55-93dd-44edb414f608",
    "employee": {
        "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
        "firstName": "John",
        "lastName": "Lennon",
        "position": "Development Manager",
        "department": "Engineering",
        "directReports": [
            {
                "employeeId": "b7839309-3348-463b-a7e3-5de1c168beb3",
                "firstName": "Paul",
                "lastName": "McCartney",
                "position": "Developer I",
                "department": "Engineering",
                "directReports": null
            },
            {
                "employeeId": "03aa1462-ffa9-4978-901b-7c001562cf6f",
                "firstName": "Ringo",
                "lastName": "Starr",
                "position": "Developer V",
                "department": "Engineering",
                "directReports": null
            }
        ]
    },
    "salary": 66000,
    "effectiveDate": "2021-02-04T01:59:15.671Z"
}
```

### [GET] Get By Employee Id - http://localhost:8080/api/reportingStructure/{employeeId}

**Request:**
```
http://localhost:8080/api/reportingStructure/16a596ae-edd3-4847-99fe-c4518e82c86f
```

**Response:**
```json
{
    "compensationId": "f4acf94a-4bf5-4e55-93dd-44edb414f608",
    "employee": {
        "employeeId": "16a596ae-edd3-4847-99fe-c4518e82c86f",
        "firstName": "John",
        "lastName": "Lennon",
        "position": "Development Manager",
        "department": "Engineering",
        "directReports": null
    },
    "salary": 66000,
    "effectiveDate": "2021-02-04T01:59:15.671Z"
}
```