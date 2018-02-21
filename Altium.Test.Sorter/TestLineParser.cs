using Altium.Test.Sorter.Api;

namespace Altium.Test.Sorter
{
  public class TestLineParser : ILineParser<TestLine>
  {
    public TestLine Parse(string data)
    {
      if (string.IsNullOrEmpty(data))
        return null;

      var parts = data.Split('.');

      int.TryParse(parts[0], out int number);

      return new TestLine(number, parts.Length < 2 ? string.Empty : parts[1]);
    }

    public string Unparse(TestLine line)
    {
      if (line == null)
        return null;

      return $"{line.Number}.{line.String}";
    }
  }
}
