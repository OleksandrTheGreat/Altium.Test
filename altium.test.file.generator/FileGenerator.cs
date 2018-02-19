using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altium.test.file.generator.api;

namespace altium.test.file.generator
{
  public class FileGenerator : IFileGenerator
  {
    private readonly IProgress<int?> _progress;

    public FileGenerator()
    {
    }
    public FileGenerator(
      IProgress<int?> progress
    )
    {
      _progress = progress;
    }

    public void Generate(
      string path,
      long size,
      int maxNumber,
      string[] values,
      int bufferSize
    )
    {
      Validate(values);
      FillFile(path, size, maxNumber, values, bufferSize);
    }

    private static void Validate(string[] values)
    {
      values = values.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

      if (values.Length == 0)
        throw new ArgumentException("String values are empty", "values");
    }

    private void FillFile(
      string path,
      long size,
      int maxNumber,
      string[] values,
      int bufferSize
    )
    {
      using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
      using (var bs = new BufferedStream(fs, bufferSize))
      {
        var random = new Random();

        var rows = GenerateRows(random, maxNumber, values, 1000);
        var len = rows.Length;

        while (fs.Position < size)
        {
          var leftSize = size - fs.Position;
          var count = leftSize < len ? leftSize : len;
          var bytes = Encoding.ASCII.GetBytes(rows, 0, (int)count);

          bs.Write(bytes, 0, (int)count);

          if (_progress != null)
            _progress.Report((int)(fs.Position * 100 / size));
        }
      }
    }

    private static string GenerateRows(
      Random random,
      int maxNumber,
      string[] values,
      int count
    )
    {
      var sb = new StringBuilder();

      for (var i = 0; i < count; i++)
        sb.Append(GenerateRow(random, maxNumber, values));

      return sb.ToString();
    }

      private static string GenerateRow(
      Random random,
      int maxNumber,
      string[] values
    )
    {
      var number = random.Next(1, maxNumber + 1);
      var idx = random.Next(0, values.Length);

      return $"{number}. {values[idx]}\n";
    }
  }
}
