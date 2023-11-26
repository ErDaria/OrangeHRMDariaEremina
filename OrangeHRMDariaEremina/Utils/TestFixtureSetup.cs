using Microsoft.Playwright;
using NUnit.Framework;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Utils;

public class TestFixtureSetup
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    protected IPage _page = null!;
    private readonly BrowserType _browserType;

    // Constructor
    public TestFixtureSetup(BrowserType browserType)
    {
        _browserType = browserType;
    }

    private async Task<IPage> RunBeforeAnyTestsAsync()
    {
        Program.Main(new[] { "install" });

        // Start driver
        var driver = new PlaywrightDriver();

        // Add launch options
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Timeout = 120_000,
            Channel = driver.GetBrowserChannel(_browserType)
        };

        // Initialize browser
        _browser = await driver.InitializeBrowser(_browserType, launchOptions);
        _context = await _browser.NewContextAsync();

        // Start tracing before creating / navigating a page.
        await _context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        // Open web page
        _page = await _context.NewPageAsync();
        await _page.GotoAsync(ConfigurationData.WebAddress);
        return _page;
    }

    [OneTimeSetUp]
    public async Task Start()
    {
        // Start web page before test
        _page = await RunBeforeAnyTestsAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        // Close browser
        // Stop tracing and export it into a zip archive.
        await _context!.Tracing.StopAsync(new()
        {
            Path = "trace.zip"
        });
        await _context!.CloseAsync();
    }
}