using altium.test.file.sorter.api;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace altium.test.file.sorter
{
  class CompressedFileRow : FileRow
  {
    public int Count { get; set; }
    public string Line { get; set; }
  }

  public class CompressedFileSorter : IFileSorter
  {
    private readonly IFileRowParser _parser;
    private readonly IProgress<int?> _parseProgress;
    private readonly IProgress<int?> _writeProgress;

    public CompressedFileSorter(
      IFileRowParser parser,
      IProgress<int?> parseProgress,
      IProgress<int?> writeProgress
    )
    {
      _parser = parser;
      _parseProgress = parseProgress;
      _writeProgress = writeProgress;
    }

    public void Sort(
      string targetFilePath,
      string sortedFilePath,
      int bufferSize
    )
    {
      var compressed = Parse(targetFilePath);
      var sorted = Sort(compressed);

      Write(
        sortedFilePath,
        sorted, 
        bufferSize
      );
    }

    private ConcurrentDictionary<string, int> Parse(string targetFilePath)
    {
      var result = new ConcurrentDictionary<string, int>();
      var totalSize = new FileInfo(targetFilePath).Length;
      long bytesRead = 0;

      foreach (var line in File.ReadLines(targetFilePath).AsParallel())
      {
        bytesRead += line.Length;

        if (_parseProgress != null)
          _parseProgress.Report((int)(bytesRead * 100 / totalSize));

        if (result.ContainsKey(line))
        {
          result[line]++;
          continue;
        }

        if (result.TryAdd(line, 1))
          continue;

        result[line]++;
      }

      if (_parseProgress != null)
        _parseProgress.Report(100);

      return result;
    }

    private IOrderedEnumerable<CompressedFileRow> Sort(ConcurrentDictionary<string, int> compressed)
    {
      return
        compressed
        .Select(x =>
        {
          var row = _parser.Parse(x.Key);

          if (row == null)
            return new CompressedFileRow
            {
              Number = 0,
              String = x.Key,
              Line = x.Key,
              Count = x.Value
            };

          return new CompressedFileRow
          {
            Number = row.Number,
            String = row.String,
            Line = x.Key,
            Count = x.Value
          };
        })
        .OrderBy(x => x.String)
        .ThenBy(x => x.Number);
    }

    private void Write(
      string sortedFilePath,
      IOrderedEnumerable<CompressedFileRow> sorted,
      int bufferSize
    )
    {
      using (var fs = new FileStream(sortedFilePath, FileMode.Create, FileAccess.Write))
      using (var bs = new BufferedStream(fs, bufferSize))
      using (var sw = new StreamWriter(bs))
      {
        var rowsWritten = 0;
        var total = sorted.Count();

        foreach (var row in sorted)
        {
          for (var i = 0; i < row.Count; i++)
            sw.WriteLine(row.Line);

          rowsWritten++;

          if (_writeProgress != null)
            _writeProgress.Report(rowsWritten * 100 / total);
        }
      }
    }
  }
}
