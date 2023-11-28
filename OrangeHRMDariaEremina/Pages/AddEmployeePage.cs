using Microsoft.Playwright;

namespace OrangeHRMDariaEremina.Pages;

public class AddEmployeePage : BasePage
{
    public AddEmployeePage(IPage page) : base(page) { }

    private ILocator tabPIM => Page.GetByRole(AriaRole.Link, new() { Name = "PIM" });
    private ILocator tabAddEmployee => Page.GetByRole(AriaRole.Link, new() { Name = "Add Employee" });

    public async Task Save() => await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

    public async Task ArrangeEmployeeId()
    {
        Random random = new Random();
        int fourDigitNumber = random.Next(1000, 1000000);
        await Page.GetByRole(AriaRole.Textbox).Last.FillAsync(fourDigitNumber.ToString());
    }

    public async Task AddImage()
    {
        await Page.Locator("form").GetByRole(AriaRole.Img, new() { Name = "profile picture" }).ClickAsync();
        var fileChooser = Page.Locator("input[type=\"file\"]");
        await fileChooser.SetInputFilesAsync("./avatar.png");
    }

}