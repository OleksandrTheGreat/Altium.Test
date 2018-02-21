using Altium.Test.Sorter.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Altium.Test.Sorter.Tests
{
  static class Utils
  {
    static ILineParser<TestLine> _parser = new TestLineParser();

    public static IEnumerable<TestLine> GenerateTestRows(int count)
    {
      var input = new List<string>();

      for (var i = 0; i < count; i++)
      {
        var String = Guid.NewGuid();

        input.Add($"{String.GetHashCode()}. {String}");
      }

      return input.Select(x => _parser.Parse(x));
    }

    public static void AssertTestRowSort(
      string title,
      ISortStrategy<TestLine> strategy,
      int blockSize
    )
    {
      var input = GenerateTestRows(blockSize);
      var expected = input.OrderBy(x => x.String).ThenBy(x => x.Number).Select(x => _parser.Unparse(x)).ToArray();

      var watch = new Stopwatch();
      watch.Start();

      var actual = strategy.Sort(input).Select(x => _parser.Unparse(x)).ToArray();
      watch.Stop();

      Assert.Equal(expected, actual);
    }
  }
}
