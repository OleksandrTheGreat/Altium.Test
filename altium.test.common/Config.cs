using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace altium.test
{
  public static class Config
  {
    private static IConfiguration _configuration;
    private static IConfiguration Configuration
    {
      get
      {
        if (_configuration != null)
          return _configuration;

        var builder = 
          new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json");

        _configuration = builder.Build();

        return _configuration;
      }
    }

    public static T GetApplicationSetting<T>(string key)
    {
      try
      {
        var value = Configuration[key];

        if (string.IsNullOrWhiteSpace(value))
          throw new Exception($"Application setting \"{key}\" is missing.");

        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch (Exception ex)
      {
        throw new Exception($"Error on getting application setting \"{key}\"", ex);
      }
    }
  }
}
