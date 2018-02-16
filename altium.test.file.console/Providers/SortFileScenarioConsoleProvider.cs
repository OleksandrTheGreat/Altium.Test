using System;
using System.Threading.Tasks;
using altium.test.file.scenarios.api;

namespace altium.test.file.console.Providers
{
  class SortFileScenarioConsoleProvider : AConsoleProvider, ISortScenarioProvider
  {
    public Task<bool> Confirm(SortScenarioSettings settings)
    {
      Console.Clear();
      xConsole.WriteHeader($"Sotring file:\n");
      Console.WriteLine($"Target file path: {settings.TargetFilePath}");
      Console.WriteLine($"Sorted file path: {settings.SortedFilePath}");
      Console.WriteLine($"Buffer size: {settings.BufferSize: #,###,##0} Bytes");

      var proceed = xConsole.Ask("Proceed? (y/n) ");

      return Task.FromResult(proceed);
    }

    public Task<SortScenarioSettings> GetSettings()
    {
      var settings = new SortScenarioSettings();

      xConsole.WriteQuestion("Enter target file path: ");
      settings.TargetFilePath = xConsole.ReadString();

      settings.SortedFilePath = $"{settings.TargetFilePath}.sorted";

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      xConsole.WriteQuestion($"Enter buffer size (default: {defaultBufferSize: #,###,###}): ");
      settings.BufferSize = xConsole.ReadInt(defaultBufferSize);

      return Task.FromResult(settings);
    }

    public void Init()
    {
      Console.Clear();

      xConsole.WriteHeader("Enter parameters to sort \"Number. String\" File:");
      Console.WriteLine();
    }
  }
}
