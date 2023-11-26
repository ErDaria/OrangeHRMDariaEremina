using Microsoft.Playwright;
using NUnit.Framework;
using OrangeHRMDariaEremina.Utils;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Tests;

[TestFixture(BrowserType.Chrome)]
[TestFixture(BrowserType.Edge)]
public class UITest : TestFixtureSetup
{

    public UITest(BrowserType browserType) : base(browserType) { }

    [Test]
    public async Task HappyPathTest()
    {
        // Verify the web page is opened
        await Assertions.Expect(_page).ToHaveTitleAsync("OrangeHRM");

        // Log in as Admin
        await _page.GetByPlaceholder("Username").FillAsync(ConfigurationData.AdminUserName);
        await _page.GetByPlaceholder("Password").FillAsync(ConfigurationData.AdminPassword);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // Verify the home page is opened
        await _page.PauseAsync();
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" })).ToBeVisibleAsync();

        // Add new employee (PIM > Add Employee)
        await _page.GetByRole(AriaRole.Link, new() { Name = "PIM" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = "Add Employee" }).ClickAsync();

        // Verify tab is opened
        await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Add Employee" })).ToBeVisibleAsync();

        // Fill-in few Personal Details
        await _page.GetByPlaceholder("First Name").FillAsync("John");
        await _page.GetByPlaceholder("Last Name").FillAsync("Doe");

        // Attach picture
        await _page.Locator("form").GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();
        await _addEmployeePage.TakeScreenshot();
        await _addEmployeePage.FileInput(".Files\\Screenshot_png.png");

        // Save the employee details
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        // Verify data has been saved
        await Assertions.Expect(_page.GetByText("Successfully Saved")).ToBeVisibleAsync();
        await Assertions.Expect(_page.GetByPlaceholder("First Name")).ToHaveValueAsync("John");
        await Assertions.Expect(_page.GetByPlaceholder("Last Name")).ToHaveValueAsync("Doe");

        await _page.GetByRole(AriaRole.Link, new() { Name = "client brand banner" }).ClickAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "Employee List" }).ClickAsync();

        await _page.GetByPlaceholder("Type for hints...").First.ClickAsync();

        await _page.GetByPlaceholder("Type for hints...").First.FillAsync("John");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();

        await _page.GetByText("0374").ClickAsync();

        await _page.GetByRole(AriaRole.Img, new() { Name = "profile picture" }).Nth(1).ClickAsync();
    }
}