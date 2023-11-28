using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using OrangeHRMDariaEremina.Utils;
using OrangeHRMDariaEremina.Utils.Models;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Tests;

[AllureFeature("Add employee UI")]
[TestFixture(BrowserType.Chrome)]
[TestFixture(BrowserType.Edge)]
public class UITest : TestFixtureSetup
{
    // placeholder for an unique employee Id
    private string employeeId = null;

    // Constructor
    public UITest(BrowserType browserType) : base(browserType) { }

    [AllureName("Open web page")]
    [AllureTag("regress", "smoke")]
    [Test, Order(1)]
    public async Task S1_OpenWebPage()
    {
        // Verify the web page is opened
        await Assertions.Expect(_page).ToHaveTitleAsync("OrangeHRM");
    }

    [AllureName("Login negative case")]
    [AllureTag("regress", "smoke")]
    [Test, Order(2)]
    public async Task S2_LoginNegativeCase()
    {
        // Log in with invalid credentials
        await _loginPage.FillWithText("Username", "Wrong name");
        await _loginPage.FillWithText("Password", "Wrong password");
        await _loginPage.ClickOnLogin();

        // Verify "Invalid credentials" message
        await Assertions.Expect(_page.GetByRole(AriaRole.Alert)).ToBeVisibleAsync();
        await Assertions.Expect(_page.GetByText("Invalid credentials")).ToBeVisibleAsync();
    }

    [AllureName("Login posiive case")]
    [AllureTag("regress", "smoke")]
    [Test, Order(3)]
    public async Task S3_LoginPositiveCase()
    {
        // Log in as Admin
        await _loginPage.FillWithText("Username", ConfigurationData.AdminUserName);
        await _loginPage.FillWithText("Password", ConfigurationData.AdminPassword);
        await _loginPage.ClickOnLogin();

        // Verify the home page is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })).ToBeVisibleAsync();
    }

    [AllureName("Add new employee negative case")]
    [AllureTag("regress", "smoke")]
    [Test, Order(4)]
    public async Task S4_AddNewEmployeeNegativeCase()
    {
        // Add new employee (PIM > Add Employee)
        await _addEmployeePage.ClickOnTab("PIM");
        await _addEmployeePage.ClickOnTab("Add Employee");

        // Verify tab is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Add Employee" })).ToBeVisibleAsync();

        // Save empty employee details
        await _addEmployeePage.Save();

        // Verify "Required" message
        await Assertions.Expect(_page.GetByText("Required").First).ToBeVisibleAsync();
    }

    [AllureName("Add new employee positive case")]
    [AllureTag("regress", "smoke")]
    // This part of test will create several users from appsettings.json file and verify that they were added in the Employee list
    [TestCaseSource(typeof(ConfigurationData), nameof(ConfigurationData.GetUsersList))]
    [Test, Order(5)]
    public async Task S5_AddNewEmployeePositiveCase(User user)
    {
        // Add new employee (PIM > Add Employee)
        await _addEmployeePage.ClickOnTab("PIM");
        await _addEmployeePage.ClickOnTab("Add Employee");

        // Fill-in few Personal Details
        await _addEmployeePage.FillWithText("First Name", user.FirstName!);
        await _addEmployeePage.FillWithText("Last Name", user.LastName!);

        // Generate random id and fill it in employee id textbox
        await _addEmployeePage.ArrangeEmployeeId();

        // Save EmployeeId
        employeeId = await _page.GetByRole(AriaRole.Textbox).Last.InputValueAsync();

        // Attach picture
        await _addEmployeePage.AddImage();

        // Save the employee details
        await _addEmployeePage.Save();

        // Verify data has been saved
        await Assertions.Expect(_page.GetByText("Successfully Saved")).ToBeVisibleAsync();
        await Assertions.Expect(_page.GetByPlaceholder("First Name")).ToHaveValueAsync(user.FirstName!);
        await Assertions.Expect(_page.GetByPlaceholder("Last Name")).ToHaveValueAsync(user.LastName!);

        // Navigate back to home screen
        await _employeeListPage.ClickOnTab("PIM");
        await _employeeListPage.ClickOnTab("Employee List");

        // Verify Employee list is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Employee Information" }))
            .ToBeVisibleAsync();

        // Search for the newly created employee details
        await _employeeListPage.SearchForEmployeeId(employeeId);

        // Verify search result
        await Assertions.Expect(_page.GetByText("(1) Record Found")).ToBeVisibleAsync();

        // Click on the search result
        await _employeeListPage.ClickOn(employeeId);

        // Verify employee details after search
        await Assertions.Expect(_page.GetByPlaceholder("First Name")).ToHaveValueAsync(user.FirstName!);
        await Assertions.Expect(_page.GetByPlaceholder("Last Name")).ToHaveValueAsync(user.LastName!);
        await Assertions.Expect(_page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Employee IdOther Id$") }).GetByRole(AriaRole.Textbox).First).ToHaveValueAsync(employeeId);
    }

    [AllureName("Delete new employee")]
    [AllureTag("regress", "smoke")]
    [TestCaseSource(typeof(ConfigurationData), nameof(ConfigurationData.GetUsersList))]
    [Test, Order(6)]
    public async Task S6_DeleteCreatedUsers(User user)
    {
        // Navigate back to Employee list screen
        await _employeeListPage.ClickOnTab("PIM");
        await _employeeListPage.ClickOnTab("Employee List");

        // Verify Employee list is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Employee Information" }))
            .ToBeVisibleAsync();

        // Search for employee
        await _employeeListPage.SearchForEmployeeName(user.FirstName, user.LastName);

        // Verify that name has been found
        await Assertions.Expect(_page.GetByRole(AriaRole.Cell, new() { Name = user.FirstName! }).First).ToBeVisibleAsync();

        // Delete user
        await _employeeListPage.DeleteUser();

        // Verify that user has been deleted
        await Assertions.Expect(_page.GetByText("Successfully Deleted")).ToBeVisibleAsync();
    }
}