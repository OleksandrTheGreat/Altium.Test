namespace Altium.Test.Scenarios.Api
{
  public interface IScenario
  {
    string Description { get; }
    void Run();
  }
}
