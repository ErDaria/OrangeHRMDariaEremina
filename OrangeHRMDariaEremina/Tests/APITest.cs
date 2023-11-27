using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using OrangeHRMDariaEremina.Utils;
using OrangeHRMDariaEremina.Utils.Models;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Tests;

[AllureNUnit]
[AllureLabel("layer", "rest")]
[AllureFeature("Labels API")]
[TestFixture]
public class APITest
{
    private IAPIRequestContext Request = null;
    private string? _state;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    protected IPage _page = null!;
    private string? _base64String;

    [AllureBefore("Test Init")]
    [SetUp]
    public async Task SetUpAPITesting()
    {
        await CreateAPIRequestContext();
    }

    private async Task CreateAPIRequestContext()
    {
        // Start driver
        var driver = new PlaywrightDriver();

        // Add launch options
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = true,
            Timeout = 120_000,
            Channel = driver.GetBrowserChannel(BrowserType.Chrome)
        };

        // Initialize browser
        _browser = await driver.InitializeBrowser(BrowserType.Chrome, launchOptions);
        _context = await _browser.NewContextAsync();

        // Open web page
        _page = await _context.NewPageAsync();
        await _page.GotoAsync(ConfigurationData.WebAddress);

        await _page.GetByPlaceholder("Username").FillAsync(ConfigurationData.AdminUserName);
        await _page.GetByPlaceholder("Password").FillAsync(ConfigurationData.AdminPassword);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // Verify the home page is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })).ToBeVisibleAsync();

        // Store session
        _state = await _context.StorageStateAsync();

        // Create playwright API context
        var _playwright = await Playwright.CreateAsync();
        Request = await _playwright.APIRequest.NewContextAsync(new() {
            BaseURL = ConfigurationData.WebAddress,
            StorageState = _state
        });
    }

    [AllureName("Login via API")]
    [AllureTag("smoke")]
    [Test, Order(1)]
    public async Task LoginTest()
    {
        // Validate that admin has been logged in
        var responseValidate = await Request.PostAsync("/web/index.php/auth/validate");

        // Validate the response
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
        var addEmployeeData = new Dictionary<string, object>();
        addEmployeeData.Add("firstName", user.FirstName);
        addEmployeeData.Add("middleName", "");
        addEmployeeData.Add("lastName", user.LastName);
        addEmployeeData.Add("empPicture", new PictureData
        {
            base64 = ConvertImageToBase64(),
            name = "avatar.png",
            type = "image/png",
            size = new FileInfo(".Files\\avatar.png").Length
        });
        addEmployeeData.Add("employeeId", fourDigitNumber.ToString());

        // Add employee
        var responsePost = await Request.PostAsync("/web/index.php/api/v2/pim/employees",
            new() { DataObject = addEmployeeData });

        // Validate the response
        Assert.True(responsePost.Ok);

        // Create parameters
        var parameters = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("nameOrId", fourDigitNumber.ToString()),
            new KeyValuePair<string, object>("includeEmployees", "onlyCurrent")
        };

        // Find the newly created employee
        var responseAdd = await Request.GetAsync("/web/index.php/api/v2/pim/employees", new() { Params = parameters });

        // Validate the response
        Assert.True(responsePost.Ok);

        var issuesJsonResponse = await responseAdd.JsonAsync();

        string empNumber = GetEmpNumber(issuesJsonResponse.Value);

        // Validate the personal details
        var responsePersonalDetails = await Request.GetAsync("/web/index.php/api/v2/pim/employees/" + empNumber);
        Assert.True(responsePersonalDetails.Ok);
    }

    [AllureAfter("Test Clean")]
    [TearDown]
    public async Task TearDownAPITesting()
    {
        await Request.DisposeAsync();
    }

    private string GetEmpNumber(JsonElement jsonElement)
    {
        string empNumber = null;

        if (jsonElement.TryGetProperty("data", out JsonElement dataElement) &&
            dataElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in dataElement.EnumerateArray())
            {
                if (item.TryGetProperty("empNumber", out JsonElement empNumberElement) &&
                    empNumberElement.ValueKind == JsonValueKind.Number)
                {
                    empNumber = empNumberElement.ToString();
                }
            }
        }

        return empNumber;
    }

    private string? ConvertImageToBase64()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "avatar.png");
        if (File.Exists(filePath))
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            _base64String = Convert.ToBase64String(imageBytes);
        }
        else
        {
            throw new FileNotFoundException($"Unable to find file at path: {filePath}");
        }

        return _base64String;
    }

}