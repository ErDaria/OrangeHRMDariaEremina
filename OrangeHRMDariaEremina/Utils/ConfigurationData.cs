using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OrangeHRMDariaEremina.Utils.Models;

namespace OrangeHRMDariaEremina.Utils;

public static class ConfigurationData
{
    private static readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    public static string WebAddress => _configuration["url"]!;
    public static string AdminUserName => _configuration["adminName"]!;
    public static string AdminPassword => _configuration["adminPassword"]!;

    public static IEnumerable<User> GetUsersList()
    {
        var users = new List<User>();
        _configuration.GetSection("Users").Bind(users);

        // Now 'users' holds the array of User objects
        foreach (var user in users)
        {
            yield return user;
        }
    }
}