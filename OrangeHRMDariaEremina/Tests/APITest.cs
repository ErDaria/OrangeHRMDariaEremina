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
        var addEmployeeData = new
        {
            firstName = user.FirstName,
            lastName = user.LastName,
            empPicture = ".Files\\Screenshot_png.png",
            employeeId = fourDigitNumber
        };

        var content = new StringContent(JsonConvert.SerializeObject(addEmployeeData), Encoding.UTF8, "application/json");

        //var response = await _client.PostAsync(ConfigurationData.WebAddress + "/api/v2/pim/employees", content);


    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await Request.DisposeAsync();
    }
}