using System;
using System.Threading.Tasks;
using Altium.Test.Scenarios.Api;

namespace Altium.Test.Providers
{
  class SortFileScenarioConsoleProvider : AConsoleProvider, ISortScenarioProvider
  {
    public Task<bool> Confirm(SortScenarioSettings settings)
    {
      Console.Clear();
      xConsole.WriteHeader($"Sotring file\n");
      Console.WriteLine($"Target file path  : {settings.TargetFilePath}");
      Console.WriteLine($"Output file path  : {settings.OutputFilePath}");
      Console.WriteLine($"Block size        : {settings.BlockSize:#,###,###} Bytes");

      var choice = xConsole.Ask("Proceed? (y/n) ");

      return Task.FromResult(choice);
    }

    public Task<SortScenarioSettings> GetSettings()
    {
      var settings = new SortScenarioSettings();

      xConsole.WriteQuestion("Enter target file path: ");
      settings.TargetFilePath = xConsole.ReadString();

      var defaultOutputPath = $"{settings.TargetFilePath}.sorted";
      xConsole.WriteQuestion($"Enter output file path (default: {defaultOutputPath}): ");
      settings.OutputFilePath = xConsole.ReadString(defaultOutputPath);

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      settings.BufferSize = defaultBufferSize;

      var defaultBlockSize = Config.GetApplicationSetting<int>("DefaultBlockSize");
      xConsole.WriteQuestion($"Enter block size (default: {defaultBlockSize:#,###,###} Bytes): ");
      settings.BlockSize = xConsole.ReadInt(defaultBlockSize);

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
