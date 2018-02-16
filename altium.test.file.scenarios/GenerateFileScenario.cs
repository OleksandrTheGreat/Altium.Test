using System;
using altium.test.file.generator.api;
using altium.test.file.scenarios.api;

namespace altium.test.file.scenarios
{
  public class GenerateFileScenario : IScenario
  {
    public string Description { get; private set; }

    private readonly IGenerateFileScanarioProvider _provider;
    private readonly IFileGenerator _generator;

    public GenerateFileScenario(
      string description,
      IGenerateFileScanarioProvider provider,
      IFileGenerator generator
    )
    {
      Description = description;
      _provider = provider;
      _generator = generator;
    }

    public async void Run()
    {
      try
      {
        _provider.Init();

        var settings = await _provider.GetSettings();
        var confirmed = await _provider.Confirm(settings);

        if (!confirmed)
        {
          _provider.Cancel();
          return;
        }

        _provider.NotifyStarted();

        var time = GenerateFile(settings);

        _provider.NotifyFinished(time);
      }
      catch(Exception ex)
      {
        _provider.NotifyError(ex);
      }
    }

    private TimeSpan GenerateFile(GenerateFileScenarioSettings settings)
    {
      var t1 = DateTime.Now;

      _generator.Generate(
        settings.FilePath,
        settings.FileSize,
        settings.MaxNumber,
        settings.Strings,
        settings.BufferSize
      );

      var t2 = DateTime.Now;

      return t2 - t1;
    }
  }
}
