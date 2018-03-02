using System;
using System.IO;
using System.Linq;
using Altium.Test.Generator.Api;
using Xunit;

namespace Altium.Test.Generator.Tests
{
  public class FileGeneratorTests
  {
    private readonly IFileGenerator _generator = new FileGenerator();

    //[Fact]
    public void Generator_should_generate_file_of_required_size()
    {
      var path = "Altium.Test.generated.txt";
      long expectedSize = 1 * 1024 * 1024;

      _generator.Generate(
        path,
        expectedSize,
        1024,
        1
      );

      var actualSize = new FileInfo(path).Length;

      Assert.Equal(expectedSize, actualSize);
    }

    [Fact]
    public void FileAdapter_CleanTrash_should_leave_input_outpud_and_remove_sort_process_files()
    {
      var inputPath = "d:/temp/10G.1";
      var outputPath = $"{inputPath}.sorted";

      inputPath = Path.GetFullPath(inputPath);
      outputPath = Path.GetFullPath(outputPath);

      for (var i = 0; i < 10; i++)
        using (var fs = File.Create($"{inputPath}.{i}")) { }

      var fa = new FileAdapter();

      fa.CleanSortTrash(inputPath, outputPath);

      var expected = new[] {
        inputPath,
        outputPath
      };

      var actual = Directory
        .GetFiles(Path.GetDirectoryName(inputPath))
        .Where(x => x.StartsWith(inputPath))
        .ToArray();

      Assert.Equal(expected, actual);
    }
  }
}
