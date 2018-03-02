using System;
using System.IO;
using System.Threading.Tasks;
using Altium.Test.Api;
using Altium.Test.Generator.Api;

namespace Altium.Test.Generator
{
  public class FileGenerator : IFileGenerator
  {
    private readonly IFileAdapter _fileAdapter;
    private readonly IFileWriter _fileWriter;
    private readonly ILineGenerator _lineGenerator;
    private static IProgress<int?> _progress;

    public FileGenerator()
    {
    }

    public FileGenerator(
      IFileAdapter fileAdapter,
      IFileWriter fileWriter,
      ILineGenerator lineGenerator,
      IProgress<int?> progress
    )
    {
      _fileAdapter = fileAdapter;
      _fileWriter = fileWriter;
      _lineGenerator = lineGenerator;
      _progress = progress;
    }

    public void Generate(
      string path,
      long size,
      int bufferSize,
      int percentOfAppearance
    )
    {
      Analyze(path, size);

      _bytesWritten = 0;

      if (bufferSize > size)
        bufferSize = (int)size;

      var workers = (int)(size / bufferSize);

      if (workers > 10)
        workers = 10;

      try
      {
        _fileAdapter.Delete(path);
        _fileWriter.BeginWrite(path, bufferSize);

        Parallel.For(0, workers, w => GenerateLines(w, bufferSize, size, percentOfAppearance));

        _fileWriter.SetLength(size);
      }
      finally
      {
        _fileWriter.EndWrite();
      }
    }

    private void Analyze(
      string path,
      long size
    )
    {
      var di = _fileAdapter.GetDriveInfo(path);

      if (di.AvailableFreeSpace < size)
        throw new InsufficientMemoryException(
          $"There is not enough available space on disk \"{di.Name}\"");
    }

    static long _bytesWritten;
    static object FILE = new Object();

    private void GenerateLines(
      int worker,
      int bufferSize,
      long bytesRequired,
      int percentOfAppearance
    )
    {
      int progress;

      using (var ms = new MemoryStream())
      using (var sw = new StreamWriter(ms))
      {
        sw.AutoFlush = true;

        while (_bytesWritten < bytesRequired)
        {
          if (ms.Length < bufferSize)
            sw.Write(_lineGenerator.Generate(percentOfAppearance, bytesRequired));
          else
          {
            lock (FILE)
            {
              ms.Position = 0;
              _fileWriter.CopyFrom(ms);
            }

            _bytesWritten += ms.Length;

            ms.SetLength(0);

            if (_progress != null)
            {
              progress = (int)(_bytesWritten * 100 / bytesRequired);

              if (progress > 100)
                progress = 100;

              _progress.Report(progress);
            }
          }
        }
      }
    }
  }
}
