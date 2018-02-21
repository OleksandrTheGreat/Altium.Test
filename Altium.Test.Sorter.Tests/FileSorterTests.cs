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

    //[Theory]
    //[InlineData(FILE_PATH)]
    //public void FileSorter_should_sort_file(string inputPath)
    //{
    //  var outputPath = $"{inputPath}.sort_test";
    //  var bufferSize = 65 * 1024 * 1024;

    //  var sorter = new FileSorter<TestLine>(
    //      new TestLineParser(),
    //      new MergeSortStrategy<TestLine>(
    //        new TestLineComparer()),
    //      new TestLineComparer(),
    //      new FileAdapter());

    //  sorter.Sort(inputPath, outputPath, bufferSize, bufferSize);

    //  Check_for_breaks(outputPath);
    //  Check_for_brocken_line(outputPath);
    //}

    [Theory]
    [InlineData(FILE_PATH)]
    public void Check_for_dups(string path)
    {
      var breaks = new List<string[]>();
      var parser = new TestLineParser();
      var comparer = new TestLineComparer();

      var lines = new List<string>();

      Parallel.ForEach(File.ReadLines(path), line => lines.Add(line));

      var grouped = lines.GroupBy(x => x).Where(x => x.Count() > 1).ToArray();
      //var grouped = lines.GroupBy(x => x).Where(x => x.Count() > 1).Take(10).ToArray();

      Assert.Empty(grouped);
    }

    //[Theory]
    //[InlineData(FILE_PATH)]
    //public void Check_for_breaks(string path)
    //{
    //  var breaks = new List<string[]>();
    //  var bufferSize = 65 * 1024 * 1024;

    //  var parser = new TestLineParser();
    //  var comparer = new TestLineComparer();

    //  long lineNumber = 1;

    //  using (var fs =
    //    new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
    //  using (var sr = new StreamReader(fs))
    //  {
    //    var line0 = parser.Parse(sr.ReadLine());

    //    while (sr.Peek() > -1)
    //    {
    //      lineNumber++;

    //      var line1 = parser.Parse(sr.ReadLine());

    //      if (comparer.Compare(line0, line1) > 0)
    //      {
    //        breaks.Add(new[] { parser.Unparse(line0), parser.Unparse(line1) });

    //        //if (breaks.Count > 9)
    //        //  break;
    //      }

    //      if (sr.Peek() > -1)
    //        line0 = parser.Parse(sr.ReadLine());
    //    }
    //  }

    //  Assert.Empty(breaks);
    //}

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
    [InlineData(@"D:\temp\1G.8")]
    public void Check_percent_of_line(string path)
    {
      var breaks = new List<string>();
      var bufferSize = 65 * 1024 * 1024;

      using (var fs =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
      using (var sr = new StreamReader(fs))
      {
        while (!sr.EndOfStream)
        {
          var line = sr.ReadLine();

          if (!breaks.Any(x => x == line))
            breaks.Add(line);
        }
      }
    }
  }
}