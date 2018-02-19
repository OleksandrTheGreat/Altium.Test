namespace altium.test.file.scenarios.api
{
  public class GenerateFileScenarioSettings
  {
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public int MaxNumber { get; set; }
    public string[] Strings { get; set; }
    public int BufferSize { get; set; }
    public int RowBlockSize { get; set; }
  }
}
