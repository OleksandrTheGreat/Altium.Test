using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Altium.Test.Generator.Api;
using Altium.Test.Scenarios.Api;

namespace Altium.Test.Scenarios
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
      catch (TaskCanceledException)
      {
        _provider.Cancel();
      }
      catch(Exception ex)
      {
        _provider.NotifyError(ex);
      }
    }

    private TimeSpan GenerateFile(GenerateFileScenarioSettings settings)
    {
      var watch = new Stopwatch();
      watch.Start();

      _generator.Generate(
        settings.FilePath,
        settings.FileSize,
        settings.BufferSize,
        settings.PercentOfAppearance
      );

      watch.Stop();

      return watch.Elapsed;
    }
  }
}
