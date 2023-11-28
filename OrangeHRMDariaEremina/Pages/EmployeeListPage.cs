using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace OrangeHRMDariaEremina.Pages;

public class EmployeeListPage : BasePage
{
    public EmployeeListPage(IPage page) : base(page) { }

    private ILocator tabPIM => Page.GetByRole(AriaRole.Link, new() { Name = "PIM" });
    private ILocator tabEmployeeList => Page.GetByRole(AriaRole.Link, new() { Name = "Employee List" });

    public async Task SearchForEmployeeId(string employeeId)
    {
        await Page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Employee Id$") })
            .GetByRole(AriaRole.Textbox).FillAsync(employeeId);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
    }

    public async Task SearchForEmployeeName(string firstName, string lastName)
    {
        await Page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Employee Name$") }).GetByRole(AriaRole.Textbox).First.ClickAsync();
        await Page.Locator("div").Filter(new() { HasTextRegex = new Regex("^Employee Name$") }).GetByRole(AriaRole.Textbox).First.FillAsync(firstName + " " + lastName);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
    }

    public async Task DeleteUser()
    {
        await Page.Locator($".oxd-icon.bi-trash").First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, Delete" }).ClickAsync();
    }
}
