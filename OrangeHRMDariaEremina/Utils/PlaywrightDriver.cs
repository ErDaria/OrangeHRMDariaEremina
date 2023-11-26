using Microsoft.Playwright;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Utils;

public class PlaywrightDriver
{
    public async Task<IBrowser> InitializeBrowser(BrowserType browser, BrowserTypeLaunchOptions launchOptions)
    {
        var playwright = await Playwright.CreateAsync();

        if (browser is BrowserType.Chrome or BrowserType.Edge)
            return await playwright.Chromium.LaunchAsync(launchOptions);

        throw new ArgumentException("Unknown browser type");
    }

    public string GetBrowserChannel(BrowserType browserType) => browserType switch
    {
        BrowserType.Chrome => "chrome",
        BrowserType.Edge => "msedge",
        _ => string.Empty
    };
}