using System.Net.Http.Headers;
using System.Text;
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
    private HttpClient _client;

    [AllureBefore("Setup API connection")]
    [OneTimeSetUp]
    public async Task Start()
    {
        _client = new HttpClient();
        Uri baseUri = new Uri(ConfigurationData.WebAddress);
        _client.BaseAddress = baseUri;

        var values = new List<KeyValuePair<string, string>>();
        values.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        var content = new FormUrlEncodedContent(values);

        var authenticationString = $"{ConfigurationData.AdminUserName}:{ConfigurationData.AdminPassword}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = content;
    }

    [AllureName("Login via API")]
    [AllureTag("smoke")]
    [Test, Order(1)]
    public async Task LoginTest()
    {
        var responseValidate = await _client.GetAsync(ConfigurationData.WebAddress);
        responseValidate.EnsureSuccessStatusCode();
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

        var response = await _client.PostAsync(ConfigurationData.WebAddress + "/api/v2/pim/employees", content);

        var responseCode = response.StatusCode;
        response.EnsureSuccessStatusCode();
    }
}