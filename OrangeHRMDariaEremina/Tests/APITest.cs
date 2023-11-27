using System.Net.Http.Headers;
using System.Text;
using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using OrangeHRMDariaEremina.Utils;
using OrangeHRMDariaEremina.Utils.Models;

namespace OrangeHRMDariaEremina.Tests;

[AllureNUnit]
[AllureLabel("layer", "rest")]
[AllureFeature("Labels API")]
[TestFixture]
public class APITest
{
    private IAPIRequestContext Request = null;

    [SetUp]
    public async Task SetUpAPITesting()
    {
        await CreateAPIRequestContext();
    }

    private async Task CreateAPIRequestContext()
    {
        var headers = new Dictionary<string, string>();

        // Set headers
        headers.Add("Username", ConfigurationData.AdminUserName);
        headers.Add("Password", ConfigurationData.AdminPassword);

        var _playwright = await Playwright.CreateAsync();
        Request = await _playwright.APIRequest.NewContextAsync(new() {
            BaseURL = ConfigurationData.WebAddress + "/web/index.php/auth/login",
            ExtraHTTPHeaders = headers,
        });
    }

    [AllureName("Login via API")]
    [AllureTag("smoke")]
    [Test, Order(1)]
    public async Task LoginTest()
    {
       // Validate that admin has been logged in
       var responseValidate = await Request.PostAsync("/web/index.php/auth/validate");
       Assert.True(responseValidate.Ok);
    }

    [AllureName("Add Employee via API")]
    [AllureTag("smoke")]
    [TestCaseSource(typeof(ConfigurationData), nameof(ConfigurationData.GetUsersList))]
    [Test, Order(2)]
    public async Task AddEmployee(User user)
    {
        // Generate random id and fill it in employee id textbox
        Random random = new Random();
        int fourDigitNumber = random.Next(1000, 10000);

        // Create body
        var addEmployeeData = new Dictionary<string, string>();
        addEmployeeData.Add("firstName", user.FirstName);
        addEmployeeData.Add("middleName", "");
        addEmployeeData.Add("lastName", user.LastName);
        addEmployeeData.Add("empPicture", null);
        addEmployeeData.Add("employeeId", fourDigitNumber.ToString());

        var response = await Request.PostAsync("/web/index.php/api/v2/pim/employees", new() { DataObject = addEmployeeData });
        var g = response.Status;
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await Request.DisposeAsync();
    }
}