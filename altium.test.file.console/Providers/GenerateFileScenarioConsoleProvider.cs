using System;
using System.Threading.Tasks;
using altium.test.file.scenarios.api;

namespace altium.test.file.console.Providers
{
  class GenerateFileScenarioConsoleProvider : AConsoleProvider, IGenerateFileScanarioProvider
  {
    private const char separator = ',';

    public Task<bool> Confirm(GenerateFileScenarioSettings settings)
    {
      Console.Clear();
      xConsole.WriteHeader($"Generating file:\n");
      Console.WriteLine($"File path: {settings.FilePath}");
      Console.WriteLine($"File size: {settings.FileSize.ToString("#,###,##0")} Bytes");
      Console.WriteLine($"Max Number: {settings.MaxNumber}");
      Console.WriteLine($"Buffer size: {settings.BufferSize:#,###,##0} Bytes");
      Console.WriteLine($"String list: {String.Join(separator, settings.Strings)}");

      var proceed = xConsole.Ask("Proceed? (y/n) ");

      return Task.FromResult(proceed);
    }

    public Task<GenerateFileScenarioSettings> GetSettings()
    {
      var settings = new GenerateFileScenarioSettings();

      Console.Clear();

      xConsole.WriteHeader("Enter parameters to generate \"Number. String\" File:");
      Console.WriteLine();

      xConsole.WriteQuestion("Enter file path: ");
      settings.FilePath = xConsole.ReadString();

      xConsole.WriteQuestion("Enter file size in bytes: ");
      settings.FileSize = xConsole.ReadLong();

      var defaultMaxNumber = Config.GetApplicationSetting<int>("DefaultMaxNumber");
      xConsole.WriteQuestion($"Enter max Number (default: {defaultMaxNumber: #,###,###}): ");
      settings.MaxNumber = xConsole.ReadInt(defaultMaxNumber);

      var defaultBufferSize = Config.GetApplicationSetting<int>("DefaultBufferSize");
      xConsole.WriteQuestion($"Enter write buffer size (default: {defaultBufferSize: #,###,###}): ");
      settings.BufferSize = xConsole.ReadInt(defaultBufferSize);

      var defaultStringValues = Config.GetApplicationSetting<string>("DefaultStringValues");
      xConsole.WriteQuestion($"Enter String list separated by \"{separator}\" (or leave empty to use default String values): ");
      settings.Strings = xConsole.ReadAnyStrings(separator, defaultStringValues);

      return Task.FromResult(settings);
    }

    public void Init()
    {
      Console.Clear();

      xConsole.WriteHeader("Enter parameters to generate \"Number. String\" File:");
      Console.WriteLine();
    }
  }
}
