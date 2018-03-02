using Altium.Test.Api;
using Altium.Test.Sorter.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Altium.Test.Sorter
{
  public class FileSorter<T> : IFileSorter
  {
    private readonly ILineParser<T> _lineParser;
    private readonly ISortStrategy<GroupedItem<T>> _sortStrategy;
    private readonly IComparer<GroupedItem<T>> _comparer;
    private readonly IFileAdapter _fileAdapter;
    private readonly IProgress<SortProgress> _progress;

    private Stopwatch _Watch;
    private bool _Done;
    private int _PassesMade;
    private int _BlocksSorted;
    private int _BlocksMerged;
    private int _RowsRed;

    public FileSorter(
      ILineParser<T> lineParser,
      ISortStrategy<GroupedItem<T>> sortStrategy,
      IComparer<GroupedItem<T>> comparer,
      IFileAdapter fileAdapter,
      IProgress<SortProgress> progress = null
    )
    {
      _lineParser = lineParser;
      _sortStrategy = sortStrategy;
      _comparer = comparer;
      _fileAdapter = fileAdapter;
      _progress = progress;
    }

    public void Sort(
      string inputPath,
      string outputPath,
      int bufferSize,
      long blockSize
    )
    {
      try
      {
        Init(outputPath);

        AnalyzeFile(
          inputPath,
          outputPath
        );

        RunFileSortTask(
          inputPath,
          inputPath,
          outputPath,
          bufferSize,
          CalculateBlockSize(inputPath, blockSize)
        );

        Wait(_Watch);
      }
      finally
      {
        _Watch.Stop();
      }
    }

    private void Init(
      string outputPath
    )
    {
      _fileAdapter.Delete(outputPath);

      _Watch = new Stopwatch();
      _Watch.Start();

      _Done = false;

      ResetProgressMarkers();
    }

    private void Wait(Stopwatch watch)
    {
      while (!_Done)
      {
        Thread.Sleep(1000);

        if (_progress != null)
          _progress.Report(new SortProgress
          {
            PassesMade = _PassesMade,
            RowsRed = _RowsRed,
            BlocksSorted = _BlocksSorted,
            BlocksMerged = _BlocksMerged,
            Elapsed = watch.Elapsed
          });
      }
    }

    private void AnalyzeFile(
      string inputPath,
      string outputPath
    )
    {
      var input = _fileAdapter.GetFileInfo(inputPath);
      var di = _fileAdapter.GetDriveInfo(outputPath);

      if (di.AvailableFreeSpace < input.Length * 3)
        throw new InsufficientMemoryException(
          $"There is not enough available space on disk \"{di.Name}\"\r\n{input.Length * 3:#,###,###} Bytes required.");
    }

    private int CalculateBlockSize(
      string inputPath,
      long blockSize
    )
    {
      try
      {
        _fileAdapter.BeginRead(inputPath, 1024);

        var linesNumber = 1000;
        var lines = _fileAdapter.ReadLines(linesNumber).ToArray();
        var averageLineSize = lines.Sum(x => x.Length + Environment.NewLine.Length) / lines.Count();

        return (int)(blockSize / averageLineSize);
      }
      finally
      {
        _fileAdapter.EndRead();
      }
    }

    private void RunFileSortTask(
      string sourcepath,
      string inputPath,
      string outputPath,
      int bufferSize,
      int blockSize
    )
    {
      Task.Factory.StartNew(() =>
      {
        try
        {
          SortFile(
            sourcepath,
            inputPath,
            outputPath,
            bufferSize,
            blockSize
          );
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex);

          _Done = true;
        }
      });
    }

    private void SortFile(
      string sourcepath,
      string inputPath,
      string outputPath,
      int bufferSize,
      int blockSize
    )
    {
      var bufferPath = $"{sourcepath}.{_PassesMade}";

      _fileAdapter.Delete(bufferPath);

      var inputfileAdapter = _fileAdapter.CreateInstance();
      var bufferFileAdapter = _fileAdapter.CreateInstance();
      var outputFileAdapter = _fileAdapter.CreateInstance();

      try
      {
        if (inputfileAdapter.GetFileInfo(inputPath).Length == 0)
        {
          if (_PassesMade > 0)
            inputfileAdapter.Delete(inputPath);

          _Done = true;
          return;
        }

        inputfileAdapter.BeginRead(inputPath, bufferSize);
        bufferFileAdapter.BeginWrite(bufferPath, bufferSize);
        outputFileAdapter.BeginWrite(outputPath, bufferSize);

        outputFileAdapter.StreamWriter.BaseStream.Position =
          outputFileAdapter.StreamWriter.BaseStream.Length;

        var output = new GroupedItem<T>[0];

        foreach (var block in inputfileAdapter.ReadBlock(blockSize))
        {
          _RowsRed += block.Count();

          var grouped = new GroupedItem<T>[0];

          if (_PassesMade == 0)
            grouped = Group(block);
          else
            grouped = Deserialize(block);

          var sorted = SortBlock(grouped);

          output = MergeBlocks(
            bufferFileAdapter,
            blockSize,
            output,
            sorted
          );
        }

        WriteUngrouped(outputFileAdapter, output);

        inputfileAdapter.EndRead();
        bufferFileAdapter.EndWrite();
        outputFileAdapter.EndWrite();

        if (_PassesMade > 0)
          inputfileAdapter.Delete(inputPath);

        _PassesMade++;
        ResetProgressMarkers();

        SortFile(
          sourcepath,
          bufferPath,
          outputPath,
          bufferSize,
          blockSize
        );
      }
      finally
      {
        inputfileAdapter.EndRead();
        bufferFileAdapter.EndWrite();
        outputFileAdapter.EndWrite();
      }
    }

    private void ResetProgressMarkers()
    {
      _BlocksSorted = 0;
      _BlocksMerged = 0;
      _RowsRed = 0;
    }

    private GroupedItem<T>[] Deserialize(
      IEnumerable<string> block
    )
    {
      return block.Select(x => Deserialize(x)).ToArray();
    }

    private GroupedItem<T>[] Group(
      IEnumerable<string> block
    )
    {
      return 
        block
        .AsParallel()
        .GroupBy(x => x)
        .Select(g => 
          new GroupedItem<T>
          {
            Count = g.Count(),
            Item = _lineParser.Parse(g.FirstOrDefault())
          }
        ).ToArray();
    }

    private GroupedItem<T>[] SortBlock(GroupedItem<T>[] block)
    {
      if (_sortStrategy.IsSorted(block))
        return block;

      block = _sortStrategy.Sort(block);

      _BlocksSorted++;

      return block;
    }

    private GroupedItem<T>[] MergeBlocks(
      IFileAdapter bufferFileAdapter,
      int blockSize,
      GroupedItem<T>[] left,
      GroupedItem<T>[] right
    )
    {
      if (left.Length == 0)
        return right;

      if (right.Length == 0)
        return left;

      if (
        _comparer.Compare(left[0], left[left.Length - 1]) == 0
        && _comparer.Compare(right[0], right[right.Length - 1]) == 0
      )
      {
        if (_comparer.Compare(left[left.Length - 1], right[0]) <= 0)
        {
          Write(bufferFileAdapter, right);

          _BlocksMerged += 2;

          return left;
        }

        Write(bufferFileAdapter, left);

        _BlocksMerged += 2;

        return right;
      }

      left = _sortStrategy.Merge(left, right);

      _BlocksMerged += 2;

      Write(bufferFileAdapter, left.Skip(blockSize).ToArray());

      return left.Take(blockSize).ToArray();
    }

    private void Write(
      IFileAdapter fileAdapter,
      GroupedItem<T>[] block
    )
    {
      foreach (var item in block)
        fileAdapter.WriteLine(Serialize(item));
    }

    private void WriteUngrouped(
      IFileAdapter fileAdapter, 
      GroupedItem<T>[] block
    )
    {
      var unparsed = block
        .Select(x => new GroupedItem<string> { Count = x.Count, Item = _lineParser.Unparse(x.Item) });

      foreach (var item in unparsed)
        for (var i = 0; i < item.Count; i++)
          fileAdapter.WriteLine(item.Item);
    }

    private const string GROUP_SEPARATOR = "·";

    private string Serialize(GroupedItem<T> item)
    {
      return $"{item.Count}{GROUP_SEPARATOR}{_lineParser.Unparse(item.Item)}";
    }

    private GroupedItem<T> Deserialize(string line)
    {
      var parts = line.Split(GROUP_SEPARATOR);

      return
        new GroupedItem<T>
        {
          Count = int.Parse(parts[0]),
          Item = _lineParser.Parse(parts[1])
        };
    }
  }
}
