using System;
using System.Threading;
using System.Threading.Tasks;
using Altium.Test.Scenarios.Api;

namespace Altium.Test.Providers
{
  class GenerateFileScenarioConsoleProvider : AConsoleProvider, IGenerateFileScanarioProvider
  {
    private const char separator = ',';

    public Task<bool> Confirm(GenerateFileScenarioSettings settings)
    {
      Console.Clear();
      xConsole.WriteHeader($"Generating file:\n");
      Console.WriteLine($"File path            : {settings.FilePath}");
      Console.WriteLine($"File size            : {settings.FileSize.ToString("#,###,##0")} Bytes");
      Console.WriteLine($"Percent of appearance: {settings.PercentOfAppearance:##0}%");

      var choice = xConsole.Ask("Proceed? (y/n) ");

      return Task.FromResult(choice);
    }

    public Task<GenerateFileScenarioSettings> GetSettings()
    {
      Console.Clear();
      xConsole.WriteHeader("Generate file");
      Console.WriteLine();

      const string template = "  {0}. {1}";

      Console.WriteLine(template, "1", "with one repeating line");
      Console.WriteLine(template, "2", "with all unique lines");
      Console.WriteLine(template, "3", "custom file");
      Console.WriteLine(template, "0", "Exit");

      switch(Console.ReadLine())
      {
        case "1":
          return GetOneLineSettings();
          
        case "2":
          return GetUniqueLineSettings();

        case "3":
          return GetCustomSettings();

        case "0":
          break;

        default:
          GetSettings();
          break;
      }

      return Task.FromCanceled<GenerateFileScenarioSettings>(new CancellationToken(true));
    }

    private Task<GenerateFileScenarioSettings> GetOneLineSettings()
    {
      var settings = new GenerateFileScenarioSettings();

      Console.Clear();

      xConsole.WriteHeader("Enter parameters to generate \"Number. String\" File:");
      Console.WriteLine();

      xConsole.WriteQuestion("Enter file path: ");
      settings.FilePath = xConsole.ReadString();

      xConsole.WriteQuestion("Enter file size in bytes: ");
      settings.FileSize = xConsole.ReadLong();

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      settings.BufferSize = defaultBufferSize;

      settings.PercentOfAppearance = 100;

      return Task.FromResult(settings);
    }

    private Task<GenerateFileScenarioSettings> GetUniqueLineSettings()
    {
      var settings = new GenerateFileScenarioSettings();

      Console.Clear();

      xConsole.WriteHeader("Enter parameters to generate \"Number. String\" File:");
      Console.WriteLine();

      xConsole.WriteQuestion("Enter file path: ");
      settings.FilePath = xConsole.ReadString();

      xConsole.WriteQuestion("Enter file size in bytes: ");
      settings.FileSize = xConsole.ReadLong();

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      settings.BufferSize = defaultBufferSize;

      settings.PercentOfAppearance = 0;

      return Task.FromResult(settings);
    }

    private Task<GenerateFileScenarioSettings> GetCustomSettings()
    {
      var settings = new GenerateFileScenarioSettings();

      Console.Clear();

      xConsole.WriteHeader("Enter parameters to generate \"Number. String\" File:");
      Console.WriteLine();

      xConsole.WriteQuestion("Enter file path: ");
      settings.FilePath = xConsole.ReadString();

      xConsole.WriteQuestion("Enter file size in bytes: ");
      settings.FileSize = xConsole.ReadLong();

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      settings.BufferSize = defaultBufferSize;

      var defaultPercentOfAppearance = Config.GetApplicationSetting<int>("DefaultPercentOfAppearance");
      xConsole.WriteQuestion($"Percent of appearance (default: {defaultPercentOfAppearance:##0}%): ");
      settings.PercentOfAppearance = xConsole.ReadInt(defaultPercentOfAppearance);

      settings.PercentOfAppearance =
        settings.PercentOfAppearance >= 0 && settings.PercentOfAppearance <= 100
        ? settings.PercentOfAppearance
        : defaultPercentOfAppearance;

      if (settings.PercentOfAppearance <= 0)
        settings.PercentOfAppearance = 1;

      return Task.FromResult(settings);
    }

    public void Init()
    {
      Console.Clear();
    }
  }
}
