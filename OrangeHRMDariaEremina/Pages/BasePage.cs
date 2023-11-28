using Microsoft.Playwright;

namespace OrangeHRMDariaEremina.Pages;

public class BasePage
{
    protected IPage Page { get; }

    public BasePage(IPage page) => Page = page;

    public async Task FillWithText(string locator, string text) => await Page.GetByPlaceholder(locator).FillAsync(text);
    public async Task ClickOnTab(string tab) => await Page.GetByRole(AriaRole.Link, new() { Name = tab }).ClickAsync();

    public async Task ClickOn(string name) => await Page.GetByText(name).ClickAsync();
}