namespace Altium.Test.Sorter.Api
{
  public class TestLine
  {
    public int Number { get; private set; }
    public string String { get; private set; }

    public TestLine(
      int number,
      string @string
    )
    {
      Number = number;
      String = @string;
    }
  }
}
