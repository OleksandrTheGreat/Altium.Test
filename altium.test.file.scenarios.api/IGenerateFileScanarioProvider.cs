using System;
using System.Threading.Tasks;

namespace altium.test.file.scenarios.api
{
  public interface IGenerateFileScanarioProvider
  {
    void Init();
    Task<GenerateFileScenarioSettings> GetSettings();
    Task<bool> Confirm(GenerateFileScenarioSettings settings);
    void Cancel();
    void NotifyError(Exception ex);
    void NotifyStarted();
    void NotifyFinished(TimeSpan time);
  }
}
