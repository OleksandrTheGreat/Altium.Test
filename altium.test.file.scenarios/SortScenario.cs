using altium.test.file.scenarios.api;
using altium.test.file.sorter.api;
using System;

namespace altium.test.file.scenarios
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
      var t1 = DateTime.Now;

      _sorter.Sort(
        settings.TargetFilePath,
        settings.SortedFilePath,
        settings.BufferSize
      );

      var t2 = DateTime.Now;

      return t2 - t1;
    }
  }
}
