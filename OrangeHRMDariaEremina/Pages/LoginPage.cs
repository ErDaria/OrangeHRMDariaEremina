using Microsoft.Playwright;

namespace OrangeHRMDariaEremina.Pages;

public class LoginPage: BasePage
{
    public LoginPage(IPage page) : base(page) { }

    private ILocator username => Page.GetByPlaceholder("Username");
    private ILocator password => Page.GetByPlaceholder("Password");
    private ILocator loginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });

    public async Task ClickOnLogin() => loginButton.ClickAsync();

}