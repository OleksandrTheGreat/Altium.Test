using Altium.Test.Sorter.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Altium.Test.Sorter.Tests
{
  public class FileSorterTests
  {
    const string FILE_PATH = @"D:\temp\1G.c";

    [Theory]
    [InlineData(FILE_PATH)]
    public void FileSorter_should_sort_file(string inputPath)
    {
      var outputPath = $"{inputPath}.sort_test";
      var bufferSize = 65 * 1024 * 1024;

      var sorter = new FileSorter<TestLine>(
          new TestLineParser(),
          new LinqSortStrategy(
            new TestLineComparer()
          ),
          new TestLineComparer(),
          new FileAdapter());

      sorter.Sort(inputPath, outputPath, bufferSize, bufferSize);

      Check_sorting(outputPath);
      Check_for_brocken_line(outputPath);
    }

    [Theory]
    [InlineData(FILE_PATH)]
    public void Check_sorting(string path)
    {
      var unsorted = new List<string[]>();
      var bufferSize = 65 * 1024 * 1024;

      var parser = new TestLineParser();
      var comparer = new TestLineComparer();

      long lineNumber = 1;

      using (var fs =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        var line0 = new GroupedItem<TestLine> { Item = parser.Parse(sr.ReadLine()) };

        while (sr.Peek() > -1)
        {
          lineNumber++;

          var line1 = new GroupedItem<TestLine> { Item = parser.Parse(sr.ReadLine()) };

          if (comparer.Compare(line0, line1) > 0)
          {
            unsorted.Add(new[] { parser.Unparse(line0.Item), parser.Unparse(line1.Item) });

            if (unsorted.Count > 9)
              break;
          }

          if (sr.Peek() > -1)
            line0 = new GroupedItem<TestLine> { Item = parser.Parse(sr.ReadLine()) };
        }
      }

      Assert.Empty(unsorted);
    }

    [Theory]
    [InlineData(FILE_PATH)]
    public void Check_for_brocken_line(string path)
    {
      var breaks = new List<string[]>();
      var bufferSize = 65 * 1024 * 1024;

      var parser = new TestLineParser();
      var comparer = new TestLineComparer();

      using (var fs =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine();

          var line = parser.Parse(data);

          if (line.String == null)
            throw new Exception($"Found broken line in {path}");
        }
      }
    }

    [Theory]
    [InlineData(@"D:\temp\10G.u")]
    public void Count_unique_lines(string path)
    {
      var duplicates = new Dictionary<string, int>();
      var lines = new List<string>();
      var bufferSize = 65 * 1024 * 1024;
      long lineCounter = 0;

      using (var fs =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        while (!sr.EndOfStream)
        {
          lineCounter++;

          var line = sr.ReadLine();

          if (!lines.Any(x => x == line))
            lines.Add(line);
          else
            if (duplicates.ContainsKey(line))
            duplicates[line]++;
          else
            duplicates.Add(line, 2);
        }
      }
    }

    [Theory]
    [InlineData(@"D:\temp\10G.u")]
    public void Check_for_duplicates(string path)
    {
      var duplicates = new List<string>();
      var bufferSize = 65 * 1024 * 1024;

      using (var fs =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        while (!sr.EndOfStream)
        {
          var line = sr.ReadLine();

          if (!duplicates.Any(x => x == line))
            duplicates.Add(line);
        }
      }

      Assert.Empty(duplicates);
    }
  }
}