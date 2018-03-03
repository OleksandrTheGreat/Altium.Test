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
    private readonly IFileReader _fileReader;
    private readonly IFileWriter _fileWriter;
    private readonly IProgress<SortProgress> _progress;

    private Stopwatch _Watch;
    private bool _Done;
    private SortStatus _Status;
    private int _PassesMade;
    private int _BlocksSorted;
    private int _BlocksMerged;
    private long _RowsRed;
    private int _RedPercent;

    public FileSorter(
      ILineParser<T> lineParser,
      ISortStrategy<GroupedItem<T>> sortStrategy,
      IComparer<GroupedItem<T>> comparer,
      IFileAdapter fileAdapter,
      IFileReader filereader,
      IFileWriter fileWriter,
      IProgress<SortProgress> progress = null
    )
    {
      _lineParser = lineParser;
      _sortStrategy = sortStrategy;
      _comparer = comparer;
      _fileAdapter = fileAdapter;
      _fileReader = filereader;
      _fileWriter = fileWriter;
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
        Init(
          inputPath,
          outputPath
        );

        AnalyzeFile(
          inputPath,
          outputPath,
          bufferSize
        );

        RunFileSortTask(
          inputPath,
          inputPath,
          outputPath,
          bufferSize,
          CalculateBlockSize(inputPath, blockSize)
        );

        Wait();
      }
      finally
      {
        _Watch.Stop();
      }
    }

    private void Init(
      string inputPath,
      string outputPath
    )
    {
      _fileAdapter.CleanSortTrash(inputPath, outputPath);

      _Watch = new Stopwatch();
      _Watch.Start();

      _Status = SortStatus.StandBy;
      _PassesMade = 0;
      _Done = false;

      ResetProgressMarkers();
    }

    private void ResetProgressMarkers()
    {
      _BlocksSorted = 0;
      _BlocksMerged = 0;
      _RowsRed = 0;
      _RedPercent = 0;
    }

    private void Wait()
    {
      while (!_Done)
      {
        Thread.Sleep(1000);

        if (_progress != null)
          _progress.Report(new SortProgress
          {
            PassesMade = _PassesMade,
            RowsRed = _RowsRed,
            RedPercent = _RedPercent,
            BlocksSorted = _BlocksSorted,
            BlocksMerged = _BlocksMerged,
            Status = _Status,
            Elapsed = _Watch.Elapsed
          });
      }
    }

    private void AnalyzeFile(
      string inputPath,
      string outputPath,
      int bufferSize
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
        _fileReader.BeginRead(inputPath, 1024);

        var linesNumber = 1000;
        var lines = _fileReader.ReadLines(linesNumber).ToArray();
        var averageLineSize = lines.Sum(x => x.Length + Environment.NewLine.Length) / lines.Count();

        return (int)(blockSize / averageLineSize) + 1;
      }
      finally
      {
        _fileReader.EndRead();
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
          _progress.Report(
            new SortProgress
            {
              Exception = ex
            });

          _Status = SortStatus.Cleaning;
          _fileAdapter.CleanSortTrash(inputPath, outputPath);

          _Done = true;
          _Status = SortStatus.StandBy;
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

      var inputfileFileReader = _fileReader.CreateInstance();
      var bufferFileWriter = _fileWriter.CreateInstance();
      var outputFileWriter = _fileWriter.CreateInstance();

      try
      {
        if (_fileAdapter.GetFileInfo(inputPath).Length == 0)
        {
          if (_PassesMade > 0)
          {
            _Status = SortStatus.Cleaning;
            _fileAdapter.Delete(inputPath);
          }

          _Done = true;
          return;
        }

        _Status = SortStatus.Cleaning;
        GC.Collect();

        inputfileFileReader.BeginRead(inputPath, bufferSize);
        bufferFileWriter.BeginWrite(bufferPath, bufferSize);
        outputFileWriter.BeginWrite(outputPath, bufferSize);

        outputFileWriter.StreamWriter.BaseStream.Position =
          outputFileWriter.StreamWriter.BaseStream.Length;

        var output = new GroupedItem<T>[0];
        long readLength = 0;
        long totalLength = inputfileFileReader.StreamReader.BaseStream.Length;

        _Status = SortStatus.Reading;

        foreach (var block in inputfileFileReader.ReadBlock(blockSize))
        {
          readLength += block.Sum(x => x.Length + Environment.NewLine.Length);
          _RowsRed += block.Count();
          _RedPercent = (int)(readLength / (double)totalLength * 100);

          var grouped = new GroupedItem<T>[0];
          var sanitized = block.Select(x => Sanitize(x));

          if (_PassesMade == 0)
            grouped = Group(sanitized);
          else
            grouped = Deserialize(sanitized);

          var sorted = SortBlock(grouped);

          output = MergeBlocks(
            bufferFileWriter,
            blockSize,
            output,
            sorted
          );

          _Status = SortStatus.Reading;
        }

        WriteUngrouped(outputFileWriter, output);

        inputfileFileReader.EndRead();
        bufferFileWriter.EndWrite();
        outputFileWriter.EndWrite();

        if (_PassesMade > 0)
        {
          _Status = SortStatus.Cleaning;
          _fileAdapter.Delete(inputPath);
        }

        _PassesMade++;
        ResetProgressMarkers();

        RunFileSortTask(
          sourcepath,
          bufferPath,
          outputPath,
          bufferSize,
          blockSize
        );
      }
      finally
      {
        inputfileFileReader.EndRead();
        bufferFileWriter.EndWrite();
        outputFileWriter.EndWrite();
      }
    }

    private GroupedItem<T>[] Deserialize(
      IEnumerable<string> block
    )
    {
      return block.Select(x => Deserialize(x)).Where(x => x != null).ToArray();
    }

    private GroupedItem<T>[] Group(
      IEnumerable<string> block
    )
    {
      _Status = SortStatus.Grouping;

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
      _Status = SortStatus.Sorting;

      if (_sortStrategy.IsSorted(block))
        return block;

      block = _sortStrategy.Sort(block);

      _BlocksSorted++;

      return block;
    }

    private GroupedItem<T>[] MergeBlocks(
      IFileWriter fileWriter,
      int blockSize,
      GroupedItem<T>[] left,
      GroupedItem<T>[] right
    )
    {
      _Status = SortStatus.Merging;

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
          Write(fileWriter, right);

          _BlocksMerged += 2;

          return left;
        }

        Write(fileWriter, left);

        _BlocksMerged += 2;

        return right;
      }

      left = _sortStrategy.Merge(left, right);

      _BlocksMerged += 2;

      Write(fileWriter, left.Skip(blockSize).ToArray());

      return left.Take(blockSize).ToArray();
    }

    private void Write(
      IFileWriter fileWriter,
      GroupedItem<T>[] block
    )
    {
      _Status = SortStatus.Writing;

      foreach (var item in block)
        fileWriter.WriteLine(Serialize(item));
    }

    private void WriteUngrouped(
      IFileWriter fileWriter, 
      GroupedItem<T>[] block
    )
    {
      _Status = SortStatus.Writing;

      var unparsed = block
        .Select(x => new GroupedItem<string> { Count = x.Count, Item = _lineParser.Unparse(x.Item) });

      foreach (var item in unparsed)
        for (var i = 0; i < item.Count; i++)
          fileWriter.WriteLine(item.Item);
    }

    private const string GROUP_SEPARATOR = "·";

    private string Serialize(GroupedItem<T> item)
    {
      return $"{item.Count}{GROUP_SEPARATOR}{_lineParser.Unparse(item.Item)}";
    }

    private GroupedItem<T> Deserialize(string line)
    {
      try
      {
        var parts = line.Split(GROUP_SEPARATOR);

        return
          new GroupedItem<T>
          {
            Count = int.Parse(parts[0]),
            Item = _lineParser.Parse(parts[1])
          };
      }
      catch(Exception ex)
      {
        //TODO: error with long line full of "\0" produced by MemoryMappedFile
      }

      return null;
    }

    private static string Sanitize(string data)
    {
      return data.Replace("\0", "");
    }
  }
}
