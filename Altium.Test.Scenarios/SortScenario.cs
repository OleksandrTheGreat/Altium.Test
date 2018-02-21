using Altium.Test.Scenarios.Api;
using Altium.Test.Sorter.Api;
using System;
using System.Diagnostics;

namespace Altium.Test.Scenarios
{
  public class SortScenario : IScenario
  {
    private readonly IFileSorter _sorter;
    private readonly ISortScenarioProvider _provider;

    public string Description { get; private set; }
    
    public SortScenario(
      string description,
      ISortScenarioProvider provider,
      IFileSorter sorter
    )
    {
      Description = description;
      _sorter = sorter;
      _provider = provider;
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

        var time = SortFile(settings);

        _provider.NotifyFinished(time);
      }
      catch(Exception ex)
      {
        _provider.NotifyError(ex);
      }
    }

    private TimeSpan SortFile(SortScenarioSettings settings)
    {
      var watch = new Stopwatch();
      watch.Start();

      _sorter.Sort(
        settings.TargetFilePath,
        settings.OutputFilePath,
        settings.BufferSize,
        settings.BlockSize
      );

      watch.Stop();

      return watch.Elapsed;
    }
  }
}
