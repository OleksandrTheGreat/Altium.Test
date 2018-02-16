namespace altium.test.file.scenarios.api
{
  public interface IScenario
  {
    string Description { get; }
    void Run();
  }
}
