﻿using Microsoft.Playwright;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OrangeHRMDariaEremina.Pages;
using BrowserType = OrangeHRMDariaEremina.Utils.Models.BrowserType;

namespace OrangeHRMDariaEremina.Utils;

[AllureNUnit]
[AllureLabel("layer", "web")]
public class TestFixtureSetup
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    protected IPage _page = null!;
    private readonly BrowserType _browserType;

    // Instances pages
    protected LoginPage _loginPage = null!;
    protected EmployeeListPage _employeeListPage = null!;
    protected AddEmployeePage _addEmployeePage = null!;

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
            Headless = true,
            Timeout = 120_000,
            Channel = driver.GetBrowserChannel(_browserType),
            SlowMo = 500
        };

        // Initialize browser
        _browser = await driver.InitializeBrowser(_browserType, launchOptions);
        _context = await _browser.NewContextAsync();

        // Open web page
        _page = await _context.NewPageAsync();
        await _page.GotoAsync(ConfigurationData.WebAddress);
        return _page;
    }

    [AllureBefore("Setup session")]
    [OneTimeSetUp]
    public async Task Start()
    {
        // Start web page before test
        _page = await RunBeforeAnyTestsAsync();
        _loginPage = new LoginPage(_page);
        _addEmployeePage = new AddEmployeePage(_page);
        _employeeListPage = new EmployeeListPage(_page);
    }

    [AllureAfter("Dispose session")]
    [OneTimeTearDown]
    public async Task TearDown()
    {
        // Close browser
        await _context!.CloseAsync();
    }
}